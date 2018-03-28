@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\99-AtivarJobServicos.ps1"
@ECHO ON
pause
