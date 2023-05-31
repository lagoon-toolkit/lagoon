echo off
echo "bin" folders
for /f %%a in ('dir /s /b bin') do rd /S /Q "%%a"
echo "obj" folders
for /f %%a in ('dir /s /b obj') do rd /S /Q "%%a"
echo "_vroot" folders
for /f %%a in ('dir /s /b _vroot') do rd /S /Q "%%a"
pause
