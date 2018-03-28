@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\000-DesativarJobServicos.ps1"
@ECHO ON
pause
