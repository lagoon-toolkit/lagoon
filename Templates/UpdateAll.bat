@echo off
for /f %%A in ('dir /s /b *.sln') do call update "%%~dpA" NP
pause
