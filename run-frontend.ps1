# PowerShell script to run the Vue.js frontend with Vite
# This script can be run from any directory

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$frontendPath = Join-Path $scriptPath "front"

Write-Host "Starting Vue.js Frontend..." -ForegroundColor Green
Write-Host "Frontend path: $frontendPath" -ForegroundColor Gray

if (-not (Test-Path $frontendPath)) {
    Write-Host "Error: Frontend path not found: $frontendPath" -ForegroundColor Red
    exit 1
}

Set-Location $frontendPath

Write-Host "Running: npm run dev" -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the frontend server" -ForegroundColor Cyan
Write-Host ""

try {
    npm run dev
}
finally {
    # Return to original directory
    Set-Location $scriptPath
}

