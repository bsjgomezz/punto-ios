# Punto iOS - Fase 1: crea la estructura de proyectos (backend .NET + frontend Next.js).
# Ejecutar desde la raiz del repositorio:  .\scripts\fase1-scaffold.ps1
# Es seguro re-ejecutarlo: se detiene si ya existen backend\ o frontend\.
$ErrorActionPreference = "Stop"

function Run($cmd) {
  Write-Host ">> $cmd" -ForegroundColor Cyan
  Invoke-Expression $cmd
  if ($LASTEXITCODE -ne 0) { throw "Fallo: $cmd (codigo $LASTEXITCODE)" }
}

if ((Test-Path "backend") -or (Test-Path "frontend")) {
  throw "Ya existe backend\ o frontend\. No se sobrescribe nada."
}

# ---------------- BACKEND ----------------
New-Item -ItemType Directory -Path "backend" | Out-Null
Push-Location backend

Run "dotnet new sln -n PuntoIOS"

Run "dotnet new webapi -n PuntoIOS.Api -o src/PuntoIOS.Api --use-controllers"
Run "dotnet new classlib -n PuntoIOS.Application -o src/PuntoIOS.Application"
Run "dotnet new classlib -n PuntoIOS.Domain -o src/PuntoIOS.Domain"
Run "dotnet new classlib -n PuntoIOS.Infrastructure -o src/PuntoIOS.Infrastructure"

foreach ($name in "UnitTests","IntegrationTests","ArchitectureTests") {
  & dotnet new xunit3 -n "PuntoIOS.$name" -o "tests/PuntoIOS.$name"
  if ($LASTEXITCODE -ne 0) { Run "dotnet new xunit -n PuntoIOS.$name -o tests/PuntoIOS.$name" }
}

# Plantillas: eliminar clases de ejemplo
Remove-Item "src/PuntoIOS.Application/Class1.cs","src/PuntoIOS.Domain/Class1.cs","src/PuntoIOS.Infrastructure/Class1.cs" -ErrorAction SilentlyContinue

# Solucion
$projects = Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object { $_.FullName }
foreach ($p in $projects) { Run "dotnet sln add `"$p`"" }

# Referencias (dependencias hacia adentro: Api -> Application -> Domain; Infrastructure -> Application)
Run "dotnet add src/PuntoIOS.Application reference src/PuntoIOS.Domain"
Run "dotnet add src/PuntoIOS.Infrastructure reference src/PuntoIOS.Application src/PuntoIOS.Domain"
Run "dotnet add src/PuntoIOS.Api reference src/PuntoIOS.Application src/PuntoIOS.Infrastructure"
Run "dotnet add tests/PuntoIOS.UnitTests reference src/PuntoIOS.Domain src/PuntoIOS.Application"
Run "dotnet add tests/PuntoIOS.IntegrationTests reference src/PuntoIOS.Api"
Run "dotnet add tests/PuntoIOS.ArchitectureTests reference src/PuntoIOS.Domain src/PuntoIOS.Application src/PuntoIOS.Infrastructure src/PuntoIOS.Api"

# Paquetes
Run "dotnet add src/PuntoIOS.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL"
Run "dotnet add src/PuntoIOS.Api package AspNetCore.HealthChecks.NpgSql"
Run "dotnet add tests/PuntoIOS.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing"
Run "dotnet add tests/PuntoIOS.IntegrationTests package Testcontainers.PostgreSql"
Run "dotnet add tests/PuntoIOS.ArchitectureTests package NetArchTest.Rules"

Run "dotnet build"
Pop-Location

# ---------------- FRONTEND ----------------
Run "npx --yes create-next-app@latest frontend --ts --eslint --tailwind --src-dir --app --import-alias `"@/*`" --use-npm --yes"
Push-Location frontend
Run "npm install -D @playwright/test vitest @vitejs/plugin-react jsdom @testing-library/react @testing-library/jest-dom"
Run "npx playwright install chromium"
Pop-Location

Write-Host "`nListo. Pegale a Claude la salida completa de este script." -ForegroundColor Green
