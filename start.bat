@echo off

call C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe -out:"4re5-injector.exe" /win32icon:icon.ico *.cs
echo compiled injector!

pause
call 4re5-injector.exe

pause
