echo "Running Pre-commit checks..."
dotnet build
echo "Checking output files..."
ls -l bin/Debug/net10.0/app.dll
