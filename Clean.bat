echo off
echo "bin" folders
for /f %%a in ('dir /s /b bin') do rd /S /Q "%%a"
echo "obj" folders
for /f %%a in ('dir /s /b obj') do rd /S /Q "%%a"
rem for /f %%a in ('dir /s /b obj') do echo "%%a"
echo "app.db" folders
del /q "%~dp0Apps\Demo\Server\app.db"
del /q "%~dp0Apps\LagoonDemo\Server\app.db"
echo "main.min.js" files
for /f %%a in ('dir /s /b main.min.js') do del /Q "%%a"
pause
