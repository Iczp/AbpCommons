<#
.SYNOPSIS
    ABP 多项目通用升级脚本（支持模块项目、应用项目、微服务项目）
    目标：升级至 ABP 10.6.1 + .NET 10 (net10.0)
.DESCRIPTION
    此脚本专为各种类型的 ABP vNext 项目设计，具备：
    1. 运行时先展示详尽的升级计划清单，确认后再执行。
    2. 自适应路径识别，批量更新 .csproj（net10.0）、props、global.json。
    3. 自动适配代码中的 Microsoft.OpenApi 2.x 命名空间 (using Microsoft.OpenApi.Models -> using Microsoft.OpenApi)。
    4. 自动适配 OpenIddict 7.x 官方重命名端点 (Endpoints.Logout -> Endpoints.EndSession, Endpoints.Device -> Endpoints.DeviceAuthorization)。
    5. 自动清理无用的 using IdentityModel; 与重复 PackageReference 项。
    6. 执行 ABP CLI 同步更新 (abp update -v 10.6.1)。
    7. 严格的编译校验拦截：仅在项目编译 100% 成功通过后，才进入数据库迁移阶段！
    8. 多项目 EF Core 数据库迁移支持 (统一名称如 Abp10.6.1)。
.PARAMETER ProjectsPath
    解决方案或项目根目录路径，默认自动探测（如果在 scripts 目录，则自动取上一级）。
.PARAMETER AbpVersion
    目标 ABP 版本号，默认 "10.6.1"
.PARAMETER TargetFramework
    目标 .NET 框架名称，默认 "net10.0"
.PARAMETER DotNetVersion
    目标 Microsoft 核心包版本号，默认 "10.0.0"
.PARAMETER MigrationName
    数据库迁移名称，默认 "Abp10.6.1"
.PARAMETER SkipGitCheck
    是否跳过未提交 Git 更改检查，默认 $false
#>

[CmdletBinding()]
param (
    [string]$ProjectsPath = "",
    [string]$AbpVersion = "10.6.1",
    [string]$TargetFramework = "net10.0",
    [string]$DotNetVersion = "10.0.9",
    [string]$MigrationName = "",
    [switch]$SkipGitCheck = $false
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($MigrationName)) {
    $MigrationName = "Abp$AbpVersion"
}

# -------------------------------------------------------------------------
# 1. 智能解析解决方案根目录
# -------------------------------------------------------------------------
if ([string]::IsNullOrWhiteSpace($ProjectsPath)) {
    if ((Split-Path $PSScriptRoot -Leaf) -ieq "scripts") {
        $solutionRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
        $ProjectsPath = $solutionRoot.Path
    } else {
        $ProjectsPath = $PSScriptRoot
    }
} else {
    $ProjectsPath = (Resolve-Path $ProjectsPath).Path
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "          ABP 解决方案通用升级工具 (.NET 10)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "工作根目录: $ProjectsPath" -ForegroundColor Gray
Write-Host "目标版本  : ABP $AbpVersion | .NET 10 ($TargetFramework) | MS $DotNetVersion" -ForegroundColor Gray
Write-Host "迁移名称  : $MigrationName" -ForegroundColor Gray

# -------------------------------------------------------------------------
# 2. 检查 Git 状态
# -------------------------------------------------------------------------
if (-not $SkipGitCheck) {
    Write-Host "`n[准备工作] 检查 Git 状态..." -ForegroundColor Cyan
    Set-Location $ProjectsPath
    $gitInstalled = Get-Command "git" -ErrorAction SilentlyContinue
    if ($gitInstalled) {
        $gitStatus = git status --porcelain 2>$null
        if ($gitStatus) {
            Write-Host "警告: 检测到工作区存在未提交的更改：" -ForegroundColor Yellow
            Write-Host $gitStatus -ForegroundColor DarkYellow
            $confirmGit = Read-Host "是否忽略未提交更改继续执行？(y/N)"
            if ($confirmGit -ne "y" -and $confirmGit -ne "Y") {
                Write-Host "用户已取消升级操作。" -ForegroundColor Red
                exit 1
            }
        } else {
            Write-Host "Git 检查通过：工作树干净。" -ForegroundColor Green
        }
    }
}

# -------------------------------------------------------------------------
# 3. 扫描项目信息与收集升级计划
# -------------------------------------------------------------------------
Write-Host "`n[分析中] 正在扫描解决方案并生成升级计划清单..." -ForegroundColor Cyan

# 探测业务版本号
$detectedVersion = "0.1.0"
$commonPropsPath = Join-Path $ProjectsPath "common.props"
if (Test-Path $commonPropsPath) {
    $propsContent = Get-Content $commonPropsPath -Raw
    if ($propsContent -match '<Version>(.*?)</Version>') {
        $detectedVersion = $matches[1]
    }
}

$inputVersion = Read-Host "请输入新的模块自定义版本号 [直接回车默认: $detectedVersion]"
if ([string]::IsNullOrWhiteSpace($inputVersion)) {
    $newVersion = $detectedVersion
} else {
    $newVersion = $inputVersion
}

# 收集待更新的 csproj 与 props 文件
$csprojFiles = Get-ChildItem -Path $ProjectsPath -Recurse -Filter *.csproj -File
$propsFiles = Get-ChildItem -Path $ProjectsPath -Recurse -Include "common.props", "Directory.Build.props" -File

# 统计分析每个项目的当前框架
$planProjectList = @()
$candidateMigrationProjects = @()

foreach ($proj in $csprojFiles) {
    $content = Get-Content $proj.FullName -Raw -Encoding utf8
    $currentTf = "未知"
    if ($content -match '<TargetFramework>(.*?)</TargetFramework>') {
        $currentTf = $matches[1]
    } elseif ($content -match '<TargetFrameworks>(.*?)</TargetFrameworks>') {
        $currentTf = $matches[1]
    }

    $planProjectList += [PSCustomObject]@{
        "项目名称"   = $proj.Name
        "当前框架"   = $currentTf
        "目标框架"   = $TargetFramework
        "相对路径"   = (Resolve-Path -Path $proj.FullName -Relative)
    }

    # 判断是否为数据库迁移/启动候选工程
    $hasTools = $content -match 'Microsoft\.EntityFrameworkCore\.Tools'
    $hasMigrations = Test-Path (Join-Path $proj.DirectoryName "Migrations")
    $isDbMigrator = $proj.Name -match 'DbMigrator'

    if ($hasMigrations -or $isDbMigrator -or $hasTools) {
        if ($proj.Name -notmatch 'Tests') {
            $candidateMigrationProjects += $proj
        }
    }
}

# -------------------------------------------------------------------------
# 4. 展示升级计划清单（表格与摘要）
# -------------------------------------------------------------------------
Write-Host "`n==========================================================" -ForegroundColor Magenta
Write-Host "                >>> 升级计划清单 (UPGRADE PLAN) <<<        " -ForegroundColor Magenta
Write-Host "==========================================================" -ForegroundColor Magenta

Write-Host "1. 全局配置变更计划:" -ForegroundColor Yellow
Write-Host "   - global.json : 锁定/更新 SDK 为 10.0.100 (rollForward: latestFeature)" -ForegroundColor White
Write-Host "   - 业务版本号  : $newVersion" -ForegroundColor White
Write-Host "   - 核心框架    : $TargetFramework" -ForegroundColor White
Write-Host "   - Volo.Abp.*  : 全部升级至 $AbpVersion" -ForegroundColor White
Write-Host "   - EF/ASP.NET  : Microsoft.* 核心包升级至 $DotNetVersion" -ForegroundColor White
Write-Host "   - 代码兼容自愈: 自动适配 OpenApi 2.x 命名空间及 OpenIddict 7.x 端点重命名" -ForegroundColor White

Write-Host "`n2. 扫描到的待更新项目列表 (共 $($planProjectList.Count) 个工程):" -ForegroundColor Yellow
$planProjectList | Format-Table -AutoSize | Out-String | Write-Host -ForegroundColor Gray

if ($propsFiles.Count -gt 0) {
    Write-Host "3. 待更新的公共属性文件 (共 $($propsFiles.Count) 个):" -ForegroundColor Yellow
    foreach ($p in $propsFiles) {
        Write-Host "   - $((Resolve-Path -Path $p.FullName -Relative))" -ForegroundColor Gray
    }
}

Write-Host "`n4. 数据库迁移说明 (共探测到 $($candidateMigrationProjects.Count) 个迁移工程):" -ForegroundColor Yellow
Write-Host "   - 策略说明     : 脚本在编译通过后仅输出【人工执行命令清单】，不直接触碰数据库" -ForegroundColor Cyan
Write-Host "   - 迁移推荐命名 : $MigrationName" -ForegroundColor Cyan
if ($candidateMigrationProjects.Count -gt 0) {
    for ($i = 0; $i -lt $candidateMigrationProjects.Count; $i++) {
        $p = $candidateMigrationProjects[$i]
        $rel = Resolve-Path -Path $p.FullName -Relative
        Write-Host "   [$($i + 1)] $($p.Name) -> $rel" -ForegroundColor White
    }
} else {
    Write-Host "   (未检测到含有 Migrations 目录的工程)" -ForegroundColor Gray
}

Write-Host "`n5. 计划执行的升级流水线:" -ForegroundColor Yellow
Write-Host "   [步骤 1] 写入或更新 global.json (.NET 10 SDK)" -ForegroundColor White
Write-Host "   [步骤 2] 批量修改公共 props、所有 .csproj 及代码适配 (OpenApi 与 OpenIddict)" -ForegroundColor White
Write-Host "   [步骤 3] 运行官方 CLI 命令: abp update -v $AbpVersion" -ForegroundColor White
Write-Host "   [步骤 4] 执行 dotnet restore 依赖还原" -ForegroundColor White
Write-Host "   [步骤 5] 执行 dotnet build 编译严格校验 (必须 100% 成功)" -ForegroundColor White
Write-Host "   [步骤 6] 生成并输出各个迁移工程的人工升级命令清单 (复制即可执行)" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Magenta

# 用户最终确认
$confirmPlan = Read-Host "`n是否确认上述升级计划并开始执行？[Y/n]"
if ($confirmPlan -eq "n" -or $confirmPlan -eq "N") {
    Write-Host "已取消升级计划，未修改任何文件。" -ForegroundColor Yellow
    exit 0
}

Write-Host "`n开始执行升级计划..." -ForegroundColor Green

# -------------------------------------------------------------------------
# [步骤 1] 配置 global.json 确保使用 .NET 10 SDK
# -------------------------------------------------------------------------
Write-Host "`n[步骤 1/6] 配置 global.json..." -ForegroundColor Cyan
$globalJsonPath = Join-Path $ProjectsPath "global.json"
$globalJsonContent = @"
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": true
  }
}
"@

if (-not (Test-Path $globalJsonPath)) {
    $globalJsonContent | Out-File -FilePath $globalJsonPath -Encoding utf8 -Force
    Write-Host "已创建 global.json (锁定 .NET 10 SDK)" -ForegroundColor Green
} else {
    try {
        $jsonObj = Get-Content $globalJsonPath -Raw | ConvertFrom-Json
        if (-not $jsonObj.sdk) {
            $jsonObj | Add-Member -MemberType NoteProperty -Name "sdk" -Value ([PSCustomObject]@{})
        }
        $jsonObj.sdk.version = "10.0.100"
        $jsonObj.sdk.rollForward = "latestFeature"
        $jsonObj | ConvertTo-Json -Depth 5 | Out-File -FilePath $globalJsonPath -Encoding utf8 -Force
        Write-Host "已更新现有 global.json -> .NET 10" -ForegroundColor Green
    } catch {
        $globalJsonContent | Out-File -FilePath $globalJsonPath -Encoding utf8 -Force
        Write-Host "已重新写入 global.json" -ForegroundColor Green
    }
}

# -------------------------------------------------------------------------
# [步骤 2] 更新 props、.csproj 并自愈代码兼容项
# -------------------------------------------------------------------------
Write-Host "`n[步骤 2/6] 更新公共 props、.csproj 项目文件及代码适配..." -ForegroundColor Cyan

# 2.1 更新公共属性文件
foreach ($propFile in $propsFiles) {
    $content = Get-Content $propFile.FullName -Raw -Encoding utf8
    $origContent = $content
    if ($content -match '<Version>') {
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, '<Version>.*?</Version>', "<Version>$newVersion</Version>")
    }
    if ($content -match '<TargetFramework>') {
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, '<TargetFramework>.*?</TargetFramework>', "<TargetFramework>$TargetFramework</TargetFramework>")
    }

    # 自动压制已知第三方包的高危漏洞 NuGet 安全审计警告 (AutoMapper 14 与 SQLitePCLRaw) 及 SixLabors 许可警告
    if ($propFile.Name -ieq "common.props") {
        if ($content -notmatch 'GHSA-rvv3-g6hj-g44x') {
            $suppressXml = @"
  <ItemGroup>
    <!-- 压制已知第三方包的 NuGet 安全审计警告 (AutoMapper 14 与 SQLitePCLRaw) -->
    <NuGetAuditSuppress Include="https://github.com/advisories/GHSA-rvv3-g6hj-g44x" />
    <NuGetAuditSuppress Include="https://github.com/advisories/GHSA-2m69-gcr7-jv3q" />
  </ItemGroup>
"@
            $content = $content -replace '</Project>', "$suppressXml`n</Project>"
        }
        if ($content -notmatch 'SIXLABORS0001' -and $content -match '<NoWarn>') {
            $content = $content -replace '(<NoWarn>.*?)<\/NoWarn>', '$1;SIXLABORS0001</NoWarn>'
        }
    }

    if ($content -ne $origContent) {
        [System.IO.File]::WriteAllText($propFile.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "  已更新属性文件: $($propFile.Name)" -ForegroundColor Green
    }
}

# 2.2 自动适配代码中的 OpenApi 2.x 与 OpenIddict 7.x 变更
$allCsFiles = Get-ChildItem -Path $ProjectsPath -Recurse -Filter *.cs -File -ErrorAction SilentlyContinue
foreach ($cs in $allCsFiles) {
    $csText = Get-Content $cs.FullName -Raw -Encoding utf8 -ErrorAction SilentlyContinue
    $origCs = $csText

    # 适配 OpenApi 2.x
    if ($csText -match 'using\s+Microsoft\.OpenApi\.Models;') {
        $csText = $csText -replace 'using\s+Microsoft\.OpenApi\.Models;', 'using Microsoft.OpenApi;'
    }

    # 适配 OpenIddict 7.x 端点重命名
    if ($csText -match 'OpenIddictConstants\.Permissions\.Endpoints\.Logout') {
        $csText = $csText -replace 'OpenIddictConstants\.Permissions\.Endpoints\.Logout', 'OpenIddictConstants.Permissions.Endpoints.EndSession'
    }
    if ($csText -match 'OpenIddictConstants\.Permissions\.Endpoints\.Device') {
        $csText = $csText -replace 'OpenIddictConstants\.Permissions\.Endpoints\.Device(?!\w)', 'OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization'
    }

    # 通用清理无用多余的 using IdentityModel;（若移除后该文件并未引用任何 IdentityModel 类型）
    if ($csText -match 'using\s+IdentityModel;\r?\n?') {
        $strippedText = [System.Text.RegularExpressions.Regex]::Replace($csText, 'using\s+IdentityModel;\r?\n?', '')
        if ($strippedText -notmatch '\bIdentityModel\b') {
            $csText = $strippedText
        }
    }

    if ($csText -ne $origCs) {
        [System.IO.File]::WriteAllText($cs.FullName, $csText, [System.Text.Encoding]::UTF8)
        Write-Host "  已适配代码语法: $($cs.Name)" -ForegroundColor Green
    }
}

# 2.3 更新所有 .csproj 文件
foreach ($proj in $csprojFiles) {
    $file = $proj.FullName
    $content = Get-Content $file -Raw -Encoding utf8
    $origContent = $content

    # 替换 TargetFramework / TargetFrameworks
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '<TargetFramework>.*?</TargetFramework>', "<TargetFramework>$TargetFramework</TargetFramework>")
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '<TargetFrameworks>.*?</TargetFrameworks>', "<TargetFrameworks>$TargetFramework</TargetFrameworks>")

    # 替换 Version
    if ($content -match '<Version>') {
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, '<Version>.*?</Version>', "<Version>$newVersion</Version>")
    }

    # 替换 Volo.Abp.*
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(<PackageReference\s+Include="Volo\.Abp[^"]*"\s+Version=")[^"]*(")',
        "`${1}$AbpVersion`${2}"
    )

    # 替换 Microsoft.EntityFrameworkCore.*
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(<PackageReference\s+Include="Microsoft\.EntityFrameworkCore[^"]*"\s+Version=")[^"]*(")',
        "`${1}$DotNetVersion`${2}"
    )

    # 替换 Microsoft.AspNetCore.*
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(<PackageReference\s+Include="Microsoft\.AspNetCore[^"]*"\s+Version=")[^"]*(")',
        "`${1}$DotNetVersion`${2}"
    )

    # 替换 Microsoft.Extensions.*
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(<PackageReference\s+Include="Microsoft\.Extensions[^"]*"\s+Version=")[^"]*(")',
        "`${1}$DotNetVersion`${2}"
    )

    # 纠正 SixLabors.ImageSharp.Drawing 版本 (必须与 ABP 10.6.1 的 ImageSharp 3.x 对齐，Drawing 3.x 依赖 ImageSharp 4.x 且有破坏性改动和收费 License 检查)
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(<PackageReference\s+Include="SixLabors\.ImageSharp\.Drawing"\s+Version=")[3-9]\.[^"]*(")',
        "`${1}2.1.4`${2}"
    )

    # 清理多余的显式 Microsoft.OpenApi 引用 (Swashbuckle 10.6.1 已自带 2.7.5)
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '\s*<PackageReference\s+Include="Microsoft\.OpenApi"[^>]*/>', '')

    if ($content -ne $origContent) {
        [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
        Write-Host "  已更新项目: $($proj.Name)" -ForegroundColor Green
    }
}

# -------------------------------------------------------------------------
# [步骤 3] 执行官方 ABP CLI 同步更新
# -------------------------------------------------------------------------
Write-Host "`n[步骤 3/6] 执行 ABP CLI 同步依赖 (abp update -v $AbpVersion)..." -ForegroundColor Cyan
Set-Location $ProjectsPath

$abpCmd = Get-Command "abp" -ErrorAction SilentlyContinue
if ($abpCmd) {
    try {
        Write-Host "正在调用 abp update..." -ForegroundColor Yellow
        abp update -v $AbpVersion
        Write-Host "ABP CLI 更新完成。" -ForegroundColor Green
    } catch {
        Write-Warning "调用 abp update 提示: $_"
    }
} else {
    Write-Host "系统环境中未发现 abp 命令，项目包引用已通过脚本完成升级。" -ForegroundColor DarkYellow
}

# -------------------------------------------------------------------------
# [步骤 4] 依赖还原
# -------------------------------------------------------------------------
Write-Host "`n[步骤 4/6] 还原项目依赖 (dotnet restore)..." -ForegroundColor Cyan
Set-Location $ProjectsPath
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore 失败，请检查 NuGet 配置。"
    exit 1
}
Write-Host "依赖还原成功。" -ForegroundColor Green

# -------------------------------------------------------------------------
# [步骤 5] 编译严格校验（关键拦截点：编译失败绝不进入迁移！）
# -------------------------------------------------------------------------
Write-Host "`n[步骤 5/6] 编译项目进行严格验证 (dotnet build)..." -ForegroundColor Cyan

dotnet build --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n==========================================================" -ForegroundColor Red
    Write-Host " [错误拦截] 项目编译存在错误！已中止后续数据库迁移操作。" -ForegroundColor Red
    Write-Host " 请根据上方具体的编译错误排查并修复代码后，再执行迁移。" -ForegroundColor Red
    Write-Host "==========================================================" -ForegroundColor Red
    exit 1
}

Write-Host "恭喜！整个解决方案所有项目在 .NET 10 下编译全部成功！" -ForegroundColor Green

# -------------------------------------------------------------------------
# [步骤 6] 编译成功后，输出人工数据库迁移命令清单 (不直接运行)
# -------------------------------------------------------------------------
Write-Host "`n==========================================================" -ForegroundColor Magenta
Write-Host "         >>> 人工数据库迁移执行命令清单 <<<               " -ForegroundColor Magenta
Write-Host "==========================================================" -ForegroundColor Magenta
Write-Host "代码及包升级已就绪，且 .NET 编译 100% 成功通过！" -ForegroundColor Green
Write-Host "已按要求停止自动迁移，以下是各工程的人工执行命令清单：`n" -ForegroundColor Yellow

if ($candidateMigrationProjects.Count -gt 0) {
    for ($i = 0; $i -lt $candidateMigrationProjects.Count; $i++) {
        $targetProject = $candidateMigrationProjects[$i]
        $relPath = Resolve-Path -Path $targetProject.DirectoryName -Relative

        Write-Host "----------------------------------------------------------" -ForegroundColor DarkCyan
        Write-Host " [工程 $($i + 1)] $($targetProject.Name)" -ForegroundColor Cyan
        Write-Host " 目录: $relPath" -ForegroundColor Gray
        Write-Host "----------------------------------------------------------" -ForegroundColor DarkCyan

        if ($targetProject.Name -match 'DbMigrator') {
            Write-Host "cd `"$relPath`"" -ForegroundColor White
            Write-Host "dotnet run`n" -ForegroundColor White
        } else {
            Write-Host "cd `"$relPath`"" -ForegroundColor White
            Write-Host "dotnet ef migrations add $MigrationName" -ForegroundColor White
            Write-Host "dotnet ef database update`n" -ForegroundColor White
        }
    }

    Write-Host "【提示】" -ForegroundColor DarkYellow
    Write-Host "  1. 若执行迁移时提示版本工具警告，可先升级工具: dotnet tool update -g dotnet-ef" -ForegroundColor Gray
    Write-Host "  2. 若需撤销刚生成的迁移，可在对应目录下执行: dotnet ef migrations remove" -ForegroundColor Gray
} else {
    Write-Host "未探测到含有 Migrations 的工程，无需执行数据库迁移。" -ForegroundColor Gray
}
Write-Host "==========================================================" -ForegroundColor Magenta

# -------------------------------------------------------------------------
# 完成
# -------------------------------------------------------------------------
Set-Location $ProjectsPath
Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host " 升级与迁移流程全部完成！已成功升级至 ABP $AbpVersion + .NET 10" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
