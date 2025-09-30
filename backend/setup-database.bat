@echo off
echo Setting up WhiskTakers Database...

echo.
echo Step 1: Creating database...
mysql -u root -p < setup-database.sql

echo.
echo Step 2: Running Entity Framework migrations...
dotnet ef database update --project WhiskTakers.Infrastructure --startup-project WhiskTakers.API

echo.
echo Database setup complete!
echo You can now start the API server with: dotnet run
pause
