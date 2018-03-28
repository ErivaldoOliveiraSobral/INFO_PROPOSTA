@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\01-Deploy WFManager.ps1"
@ECHO ON
pause
