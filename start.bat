@echo off

REM COMPILE 4re5 obfuscator
call C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe -out:"4re5-obfuscator.exe" *.cs
echo compiled obfuscator!

pause
REM OBFUSCATE files in /testing/ without base64 encoding of strings
call 4re5-obfuscator.exe testing false

REM OBFUSCATE files in /testing/ with base64 encoding of strings
REM call 4re5-obfuscator.exe testing true


REM TESTING output
cd Obfuscated
REM COMPILE obfuscated files
call C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe -out:"test.exe" *.cs 
call test.exe

REM GO back to root
cd .. 


pause
