Write-Host "Setting up WhiskTakers Database..." -ForegroundColor Green

Write-Host ""
Write-Host "Step 1: Creating database..." -ForegroundColor Yellow
mysql -u root -p < setup-database.sql

Write-Host ""
Write-Host "Step 2: Running Entity Framework migrations..." -ForegroundColor Yellow
dotnet ef database update --project WhiskTakers.Infrastructure --startup-project WhiskTakers.API

Write-Host ""
Write-Host "Database setup complete!" -ForegroundColor Green
Write-Host "You can now start the API server with: dotnet run" -ForegroundColor Cyan
Read-Host "Press Enter to continue"
