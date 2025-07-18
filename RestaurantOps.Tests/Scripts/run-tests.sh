#!/bin/bash

# Restaurant Operations - Test Runner with Code Coverage
# This script runs all tests and generates code coverage reports

echo "🧪 Running Restaurant Operations Tests with Code Coverage..."

# Clean previous results
rm -rf TestResults/
rm -rf coverage/

# Restore packages
echo "📦 Restoring packages..."
dotnet restore

# Build the solution
echo "🏗️  Building solution..."
dotnet build --no-restore

# Run tests with coverage
echo "🧪 Running tests with coverage..."
dotnet test \
  --no-build \
  --settings coverlet.runsettings \
  --collect:"XPlat Code Coverage" \
  --logger "trx;LogFileName=test-results.trx" \
  --results-directory ./TestResults/

# Generate HTML coverage report
echo "📊 Generating HTML coverage report..."
dotnet tool install -g dotnet-reportgenerator-globaltool --ignore-failed-sources 2>/dev/null || true

# Find the coverage file
COVERAGE_FILE=$(find ./TestResults -name "coverage.cobertura.xml" | head -1)

if [ -f "$COVERAGE_FILE" ]; then
    reportgenerator \
        -reports:"$COVERAGE_FILE" \
        -targetdir:"./TestResults/coverage-html" \
        -reporttypes:"Html;TextSummary"
    
    echo "✅ Coverage report generated at: ./TestResults/coverage-html/index.html"
    
    # Display summary
    echo ""
    echo "📈 Coverage Summary:"
    cat ./TestResults/coverage-html/Summary.txt
else
    echo "❌ Coverage file not found"
fi

echo ""
echo "🏁 Test run completed!"
echo "📁 Test results: ./TestResults/"
echo "📊 Coverage report: ./TestResults/coverage-html/index.html"