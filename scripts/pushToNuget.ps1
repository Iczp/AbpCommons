$defaultVersion = "0.9.0.903"

$solutionFiles = Get-Item -Path ".\*.sln"

$solutionName = [System.IO.Path]::GetFileNameWithoutExtension($solutionFiles.FullName)

$pkg = $solutionName
$nugetKeyFilePath = "../nuget_apikey.txt" 
$nugetSource = "https://api.nuget.org/v3/index.json" # NuGet 推送地址

Write-Host "发现: $($solutionFiles.Name)" -ForegroundColor Yellow


$newVersion = Read-Host "请输入新的版本号 (例如 $defaultVersion)"
if (-not $newVersion) {
    $newVersion = $defaultVersion
    Write-Host "使用版本号: $newVersion" -ForegroundColor Yellow
    # exit 1
}

Get-ChildItem -Path $projectsPath -Recurse -Filter *.csproj | ForEach-Object {
    $file = $_.FullName
    $fileName = $_.Name
    # 更新版本号
    if ((Get-Content $file) -match '<Version>') {
        (Get-Content $file) -replace '<Version>.*<\/Version>', "<Version>$newVersion</Version>" | Set-Content $file
        Write-Host "已更新版本号[$newVersion]: $fileName" -ForegroundColor Green
    } 
}

# 获取当前目录下的所有 .sln 文件
$response = Read-Host "是否重新构建解决方案 $($solutionFiles.Name)?(y/n)" 

if ([string]::IsNullOrWhiteSpace($response)) {
    $response = "y"
}

if ($response -eq "y" -or $response -eq "Y") {

    Write-Host "dotnet build -c Release" -ForegroundColor Yellow

    # 遍历所有找到的解决方案文件并进行构建
    foreach ($solutionFile in $solutionFiles) {
        Write-Host "dotnet build -c Release $($solutionFile.FullName)" -ForegroundColor Cyan
        dotnet build -c Release $solutionFile.FullName
        
        # 检查构建是否成功
        if ($LASTEXITCODE -ne 0) {
            Write-Error "构建失败，退出代码：$LASTEXITCODE，解决方案文件：$($solutionFile.FullName)"
        }
        else {
            Write-Host "构建成功：$($solutionFile.FullName)"
        }
    }
    Write-Host "项目构建成功！" -ForegroundColor Green
}
else {
    Write-Host "跳过构建" -ForegroundColor Yellow
}


Write-Host "查找 *$newVersion.nupkg 文件:" -ForegroundColor Cyan

Get-ChildItem -Path $projectsPath -Recurse -Filter *$newVersion.nupkg | ForEach-Object {
    $nupkgFile = $_.Name
    Write-Host "nupkg: $nupkgFile" -ForegroundColor Cyan
}

$confirmPush = Read-Host "是否推送版本[ $newVersion ]到 NuGet?(y/n)"
# dotnet nuget push "../src/*/bin/Release/*$newVersion.nupkg" --skip-duplicate -k $nugetKeyFilePath --source $nugetSource

# 如果用户没有输入或者输入的是空字符串，则默认设置为 "y"
if ([string]::IsNullOrWhiteSpace($confirmPush)) {
    $confirmPush = "y"
}

if (!$confirmPush -eq "y") {
    Write-Host "推送到 NuGet 源已取消。" -ForegroundColor Yellow
    exit 1
}

# 从文件读取 NuGet API Key
if (Test-Path $nugetKeyFilePath) {
    $nugetApiKey = Get-Content $nugetKeyFilePath -ErrorAction Stop
    Write-Host "已成功读取 NuGet API Key: ******" -ForegroundColor Green
    Write-Host "推送地址: $nugetSource" -ForegroundColor Green
}
else {
    Write-Error "未找到 NuGet API Key 文件，请检查路径：$nugetKeyFilePath"

    $nugetApiKey = Read-Host "请输入NuGet API Key:" 

    if ([string]::IsNullOrWhiteSpace($nugetApiKey)) {
        Write-Error "NuGet API Key为空."
        exit 0
    }
}

Write-Host "开始推送到 NuGet 源:$nugetSource" -ForegroundColor Cyan
# dotnet nuget push "../src/*/bin/Release/*$newVersion.nupkg" --skip-duplicate -k $nugetKeyFilePath --source $nugetSource

$nupkgFiles = Get-ChildItem -Path $projectsPath -Recurse -Filter *$newVersion.nupkg
$totalFiles = $nupkgFiles.Count
$index = 0
$success = 0
$nupkgFiles | ForEach-Object {
    $nupkgFile = $_.FullName
    $nupkgFileName = $_.Name
    $index++
    Write-Host "[$index/$totalFiles]dotnet nuget push $nupkgFileName" -ForegroundColor Cyan
    dotnet nuget push $nupkgFile --api-key $nugetApiKey --skip-duplicate --source $nugetSource
    if ($?) {
        $success++
        Write-Host "[$index/$totalFiles]推送成功 $nupkgFileName" -ForegroundColor Green
    }
    else {
        Write-Error "[$index/$totalFiles]推送失败: $nupkgFile" 
        # exit 1
    }
}
if ($success = $totalFiles) {
    Write-Host "所有包[$totalFiles]已成功推送到 NuGet 源。" -ForegroundColor Green
}
else {
    Write-Error "有 $($totalFiles-$success) 个包未推送成功."
}
Write-Host "查看 https://www.nuget.org/packages?q=$pkg" -ForegroundColor Green
