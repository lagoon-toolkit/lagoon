@echo off
SET TEMPLATE_PATH=%~1
rem Remove the trailing slash
IF %TEMPLATE_PATH:~-1%==\ SET TEMPLATE_PATH=%TEMPLATE_PATH:~0,-1%
dotnet new uninstall %TEMPLATE_PATH%
IF NOT %2.==NP. pause