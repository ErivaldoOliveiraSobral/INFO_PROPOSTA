@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\01-ApagarListasExistentes.ps1"
@ECHO ON
pause
