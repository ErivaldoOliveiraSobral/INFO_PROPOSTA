@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\00-BKP_Listas.ps1"
@ECHO ON
pause
