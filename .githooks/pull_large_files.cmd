@echo off
setlocal
chcp 65001 >nul
set "git_bash=%ProgramFiles%\Git\bin\bash.exe"
if exist "%git_bash%" goto run
set "git_bash=%LocalAppData%\Programs\Git\bin\bash.exe"
if exist "%git_bash%" goto run
set "git_bash=%ProgramFiles(x86)%\Git\bin\bash.exe"
if exist "%git_bash%" goto run

echo Git Bash was not found. Install Git for Windows and try again.
pause
exit /b 1

:run
"%git_bash%" "%~dp0pull_large_files.sh"
set "result=%errorlevel%"
echo.
if "%result%"=="0" (
    echo Completed successfully.
) else (
    echo Failed with exit code %result%. See the error above.
)
pause
exit /b %result%
