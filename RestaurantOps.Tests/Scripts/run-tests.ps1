# Restaurant Operations - Test Runner with Code Coverage (PowerShell)
# This script runs all tests and generates code coverage reports

Write-Host "🧪 Running Restaurant Operations Tests with Code Coverage..." -ForegroundColor Green

# Clean previous results
if (Test-Path "TestResults") { Remove-Item -Recurse -Force "TestResults" }
if (Test-Path "coverage") { Remove-Item -Recurse -Force "coverage" }

# Restore packages
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build the solution
Write-Host "🏗️  Building solution..." -ForegroundColor Yellow
dotnet build --no-restore

# Run tests with coverage
Write-Host "🧪 Running tests with coverage..." -ForegroundColor Yellow
dotnet test `
  --no-build `
  --settings coverlet.runsettings `
  --collect:"XPlat Code Coverage" `
  --logger "trx;LogFileName=test-results.trx" `
  --results-directory ./TestResults/

# Generate HTML coverage report
Write-Host "📊 Generating HTML coverage report..." -ForegroundColor Yellow

# Install report generator if not present
try {
    dotnet tool install -g dotnet-reportgenerator-globaltool --ignore-failed-sources 2>$null
} catch {
    # Tool already installed
}

# Find the coverage file
$coverageFile = Get-ChildItem -Path "./TestResults" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1

if ($coverageFile) {
    reportgenerator `
        -reports:"$($coverageFile.FullName)" `
        -targetdir:"./TestResults/coverage-html" `
        -reporttypes:"Html;TextSummary"
    
    Write-Host "✅ Coverage report generated at: ./TestResults/coverage-html/index.html" -ForegroundColor Green
    
    # Display summary
    Write-Host ""
    Write-Host "📈 Coverage Summary:" -ForegroundColor Cyan
    Get-Content "./TestResults/coverage-html/Summary.txt"
} else {
    Write-Host "❌ Coverage file not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "🏁 Test run completed!" -ForegroundColor Green
Write-Host "📁 Test results: ./TestResults/" -ForegroundColor White
Write-Host "📊 Coverage report: ./TestResults/coverage-html/index.html" -ForegroundColor White