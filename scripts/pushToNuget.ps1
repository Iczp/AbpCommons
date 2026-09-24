# -------------------------------------------------------------------------
# NuGet 推送脚本 (支持自动探测解决方案、版本同步、打包与发布)
# -------------------------------------------------------------------------

$ErrorActionPreference = "Stop"

# 1. 智能定位工作根目录与解决方案
if ((Split-Path $PSScriptRoot -Leaf) -ieq "scripts") {
    $solutionRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
} else {
    $solutionRoot = (Resolve-Path ".").Path
}

$projectsPath = $solutionRoot
Set-Location $solutionRoot

$solutionFiles = Get-ChildItem -Path $solutionRoot -Filter "*.sln" -File
if ($solutionFiles.Count -eq 0) {
    Write-Error "在目录 '$solutionRoot' 下未找到任何 .sln 解决方案文件。"
    exit 1
}

$solutionFile = $solutionFiles[0]
$solutionName = [System.IO.Path]::GetFileNameWithoutExtension($solutionFile.FullName)
$pkg = $solutionName
$nugetKeyFilePath = Join-Path $solutionRoot "nuget_apikey.txt"
$nugetKeyParentPath = Join-Path (Split-Path $solutionRoot -Parent) "nuget_apikey.txt"
$nugetSource = "https://api.nuget.org/v3/index.json"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "                ABP 模块 NuGet 发布工具" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "工作根目录  : $solutionRoot" -ForegroundColor Gray
Write-Host "解决方案    : $($solutionFile.Name)" -ForegroundColor Yellow

# 2. 探测当前版本号
$defaultVersion = "0.10.0"
$commonPropsPath = Join-Path $solutionRoot "common.props"
if (Test-Path $commonPropsPath) {
    $propsContent = Get-Content $commonPropsPath -Raw -Encoding utf8
    if ($propsContent -match '<Version>(.*?)</Version>') {
        $defaultVersion = $matches[1]
    }
}

$inputVersion = Read-Host "请输入新的版本号 [直接回车使用: $defaultVersion]"
if ([string]::IsNullOrWhiteSpace($inputVersion)) {
    $newVersion = $defaultVersion
} else {
    $newVersion = $inputVersion
}
Write-Host "当前目标版本号: $newVersion" -ForegroundColor Green

# 3. 同步版本号到 common.props 及所有 .csproj
if (Test-Path $commonPropsPath) {
    $propsText = Get-Content $commonPropsPath -Raw -Encoding utf8
    if ($propsText -match '<Version>') {
        $updatedProps = [System.Text.RegularExpressions.Regex]::Replace($propsText, '<Version>.*?</Version>', "<Version>$newVersion</Version>")
        [System.IO.File]::WriteAllText($commonPropsPath, $updatedProps, [System.Text.Encoding]::UTF8)
        Write-Host "已更新公共属性版本号: common.props" -ForegroundColor Green
    }
}

Get-ChildItem -Path $projectsPath -Recurse -Filter *.csproj | ForEach-Object {
    $file = $_.FullName
    $fileName = $_.Name
    $content = Get-Content $file -Raw -Encoding utf8
    if ($content -match '<Version>') {
        $updatedContent = [System.Text.RegularExpressions.Regex]::Replace($content, '<Version>.*?</Version>', "<Version>$newVersion</Version>")
        [System.IO.File]::WriteAllText($file, $updatedContent, [System.Text.Encoding]::UTF8)
        Write-Host "已更新版本号[$newVersion]: $fileName" -ForegroundColor Green
    }
}

# 4. 构建解决方案
$response = Read-Host "`n是否重新构建解决方案 $($solutionFile.Name)? [Y/n]"
if ([string]::IsNullOrWhiteSpace($response)) {
    $response = "y"
}

if ($response -eq "y" -or $response -eq "Y") {
    Write-Host "正在执行 Release 编译构建..." -ForegroundColor Yellow
    dotnet build -c Release $solutionFile.FullName

    if ($LASTEXITCODE -ne 0) {
        Write-Error "构建失败，退出代码：$LASTEXITCODE"
        exit 1
    }
    Write-Host "项目构建成功！" -ForegroundColor Green
} else {
    Write-Host "跳过构建步骤。" -ForegroundColor Yellow
}

# 5. 扫描待推送的 nupkg 包 (排除 obj 目录及 symbols 包)
Write-Host "`n查找待推送的 *$newVersion.nupkg 包..." -ForegroundColor Cyan

$nupkgFiles = Get-ChildItem -Path $projectsPath -Recurse -Filter "*$newVersion.nupkg" |
    Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\.symbols\.nupkg$' }

if ($nupkgFiles.Count -eq 0) {
    Write-Error "未找到匹配版本 *$newVersion.nupkg 的 NuGet 包文件！请确认是否已成功打包。"
    exit 1
}

Write-Host "扫描到以下 $($nupkgFiles.Count) 个 NuGet 包:" -ForegroundColor Yellow
foreach ($pkgItem in $nupkgFiles) {
    Write-Host "  - $($pkgItem.Name)" -ForegroundColor Gray
}

# 6. 确认推送
$confirmPush = Read-Host "`n是否确认推送上述 $($nupkgFiles.Count) 个包到 NuGet? [Y/n]"
if ([string]::IsNullOrWhiteSpace($confirmPush)) {
    $confirmPush = "y"
}

if ($confirmPush -ne "y" -and $confirmPush -ne "Y") {
    Write-Host "已取消推送到 NuGet 源。" -ForegroundColor Yellow
    exit 0
}

# 7. 读取或输入 NuGet API Key
$nugetApiKey = ""
if (Test-Path $nugetKeyFilePath) {
    $nugetApiKey = (Get-Content $nugetKeyFilePath -Raw -ErrorAction SilentlyContinue).Trim()
} elseif (Test-Path $nugetKeyParentPath) {
    $nugetApiKey = (Get-Content $nugetKeyParentPath -Raw -ErrorAction SilentlyContinue).Trim()
}

if ([string]::IsNullOrWhiteSpace($nugetApiKey)) {
    Write-Host "未探测到本地 nuget_apikey.txt，请手动输入：" -ForegroundColor DarkYellow
    $nugetApiKey = Read-Host "请输入 NuGet API Key"
    if ([string]::IsNullOrWhiteSpace($nugetApiKey)) {
        Write-Error "NuGet API Key 不能为空，操作已终止。"
        exit 1
    }
} else {
    Write-Host "已自动读取 NuGet API Key: ******" -ForegroundColor Green
}

# 8. 依次推送各个包
Write-Host "`n开始推送到 NuGet 源: $nugetSource" -ForegroundColor Cyan

$totalFiles = $nupkgFiles.Count
$index = 0
$success = 0

foreach ($item in $nupkgFiles) {
    $index++
    $nupkgFile = $item.FullName
    $nupkgFileName = $item.Name
    Write-Host "[$index/$totalFiles] 正在推送: $nupkgFileName" -ForegroundColor Cyan

    dotnet nuget push $nupkgFile --api-key $nugetApiKey --skip-duplicate --source $nugetSource

    if ($LASTEXITCODE -eq 0) {
        $success++
        Write-Host "[$index/$totalFiles] 推送成功: $nupkgFileName" -ForegroundColor Green
    } else {
        Write-Warning "[$index/$totalFiles] 推送失败: $nupkgFileName"
    }
}

Write-Host "`n==========================================================" -ForegroundColor Cyan
if ($success -eq $totalFiles) {
    Write-Host "所有包 [$totalFiles] 已全部成功推送到 NuGet 源！" -ForegroundColor Green
} else {
    Write-Warning "推送完成：成功 $success 个，未成功 $($totalFiles - $success) 个。"
}
Write-Host "查看已发布的包: https://www.nuget.org/packages?q=$pkg" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Cyan
