# Build Release Single-File Executable
# This script builds a production-ready, single-file, Native AOT, self-contained executable
# Usage: .\build-release.ps1
# After build, enter '1' to run the built project, or any other key to exit

$ErrorActionPreference = "Stop"

# Debug information
$buildStartTime = Get-Date
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Release Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Start time: $($buildStartTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
Write-Host ""

$projectPath = "back\Plumsail.Interview.Web\Plumsail.Interview.Web.csproj"
$outputPath = "back\publish-release"

# Display build configuration
Write-Host "Build Configuration:" -ForegroundColor Yellow
Write-Host "  Project: $projectPath" -ForegroundColor Gray
Write-Host "  Output: $outputPath" -ForegroundColor Gray
Write-Host "  Configuration: Release" -ForegroundColor Gray
Write-Host "  Target: Single-file, Native AOT (trimming enabled), Self-contained" -ForegroundColor Gray

# Check .NET SDK version
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion) {
        Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Gray
    } else {
        Write-Host "  .NET SDK: Unable to determine" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  .NET SDK: Unable to determine" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Building release single-file executable..." -ForegroundColor Green

# Check if project file exists
if (-not (Test-Path $projectPath)) {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error: Project file not found!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "`nError Details:" -ForegroundColor Yellow
    Write-Host "  Expected path: $projectPath" -ForegroundColor Gray
    Write-Host "  Current directory: $(Get-Location)" -ForegroundColor Gray
    exit 1
}

# Clean previous build
try {
    if (Test-Path $outputPath) {
        Write-Host "Cleaning previous build..." -ForegroundColor Yellow
        Remove-Item -Path $outputPath -Recurse -Force -ErrorAction SilentlyContinue
    }
} catch {
    Write-Host "Error cleaning previous build:" -ForegroundColor Red
    Write-Host "  Message: $($_.Exception.Message)" -ForegroundColor Yellow
    exit 1
}

# Build and publish
Write-Host "`nPublishing release build..." -ForegroundColor Cyan
$publishStartTime = Get-Date

try {
    $publishOutput = dotnet publish $projectPath `
        -c Release `
        -o $outputPath `
        --verbosity minimal 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n========================================" -ForegroundColor Red
        Write-Host "Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "========================================" -ForegroundColor Red
        Write-Host "`nBuild output:" -ForegroundColor Yellow
        $publishOutput | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
        exit 1
    }
} catch {
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "Build failed with exception!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  Message: $($_.Exception.Message)" -ForegroundColor Yellow
    exit 1
}

$publishEndTime = Get-Date
$publishDuration = $publishEndTime - $publishStartTime
Write-Host "  Build completed in $($publishDuration.TotalSeconds.ToString('F2')) seconds" -ForegroundColor Gray

# Display build results
$buildEndTime = Get-Date
$totalDuration = $buildEndTime - $buildStartTime

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Total build time: $($totalDuration.TotalSeconds.ToString('F2')) seconds" -ForegroundColor Gray
Write-Host "End time: $($buildEndTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
Write-Host ""
# Play beep sound to indicate build completion
[Console]::Beep()

$exePath = Join-Path $outputPath "Plumsail.Interview.Web.exe"
try {
    if (Test-Path $exePath) {
        $fileInfo = Get-Item $exePath
        $sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
        $sizeBytes = $fileInfo.Length
        
        # Get output directory info
        $outputFiles = Get-ChildItem -Path $outputPath -File
        $outputFileCount = $outputFiles.Count
        $totalOutputSize = ($outputFiles | Measure-Object -Property Length -Sum).Sum
        $totalOutputSizeMB = [math]::Round($totalOutputSize / 1MB, 2)
        
        Write-Host "Build Output Information:" -ForegroundColor Yellow
        Write-Host "  Output location: $outputPath" -ForegroundColor Gray
        Write-Host "  Files in output: $outputFileCount" -ForegroundColor Gray
        Write-Host "  Total output size: $totalOutputSizeMB MB" -ForegroundColor Gray
        Write-Host "  Executable size: $sizeMB MB ($sizeBytes bytes)" -ForegroundColor Cyan
        Write-Host "  Executable path: $exePath" -ForegroundColor Gray
        Write-Host ""
        
        # Prompt user for input
        Write-Host ""
        Write-Host "Enter '1' to run the built project, or any other key to exit: " -ForegroundColor Yellow -NoNewline
        $userInput = Read-Host
        
        if ($userInput -eq "1") {
            Write-Host "`nStarting application..." -ForegroundColor Green
            Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Cyan
            Write-Host ""
            try {
                & $exePath
            } catch {
                Write-Host "`nError running application:" -ForegroundColor Red
                Write-Host "  Message: $($_.Exception.Message)" -ForegroundColor Yellow
            }
        } else {
            Write-Host "Exiting..." -ForegroundColor Gray
        }
    } else {
        Write-Host "========================================" -ForegroundColor Yellow
        Write-Host "Warning: Executable not found!" -ForegroundColor Yellow
        Write-Host "========================================" -ForegroundColor Yellow
        Write-Host "Expected path: $exePath" -ForegroundColor Gray
        Write-Host "Output directory exists: $(Test-Path $outputPath)" -ForegroundColor Gray
        if (Test-Path $outputPath) {
            $actualFiles = Get-ChildItem -Path $outputPath -File | Select-Object -First 10 Name
            Write-Host "Files in output directory:" -ForegroundColor Gray
            foreach ($file in $actualFiles) {
                Write-Host "  - $($file.Name)" -ForegroundColor Gray
            }
        }
        Write-Host "`nPress any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
} catch {
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "Error processing build output!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  Message: $($_.Exception.Message)" -ForegroundColor Yellow
    exit 1
}

