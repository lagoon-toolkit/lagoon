@echo off
SET SOLUTION_PATH=%~1
rem Remove the trailing slash
IF %SOLUTION_PATH:~-1%==\ SET SOLUTION_PATH=%SOLUTION_PATH:~0,-1%
call lgn update --prerelease %SOLUTION_PATH%
IF NOT %2.==NP. pause