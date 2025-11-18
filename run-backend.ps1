# PowerShell script to run the .NET backend
# This script can be run from any directory

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $scriptPath "back\Plumsail.Interview.Web"

Write-Host "Starting .NET Backend..." -ForegroundColor Green
Write-Host "Backend path: $backendPath" -ForegroundColor Gray

if (-not (Test-Path $backendPath)) {
    Write-Host "Error: Backend path not found: $backendPath" -ForegroundColor Red
    exit 1
}

Set-Location $backendPath

Write-Host "Running: dotnet run" -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the backend server" -ForegroundColor Cyan
Write-Host ""

try {
    dotnet run
}
finally {
    # Return to original directory
    Set-Location $scriptPath
}

