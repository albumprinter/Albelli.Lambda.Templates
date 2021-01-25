@echo off
cls
pushd "%~dp0"
call paket restore
if errorlevel 1 (
  exit /b %errorlevel%
)

dotnet tool restore
dotnet fake run build.fsx --parallel 1
popd