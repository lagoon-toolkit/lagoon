@echo off
for /f %%A in ('dir /s /b .template.config') do call "%~dp0Install.bat" "%%~dpA" NP
pause
