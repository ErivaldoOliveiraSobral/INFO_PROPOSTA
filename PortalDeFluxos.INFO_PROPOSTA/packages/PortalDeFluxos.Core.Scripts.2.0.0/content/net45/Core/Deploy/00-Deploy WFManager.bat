@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\00-Deploy WFManager.ps1"
@ECHO ON
pause
