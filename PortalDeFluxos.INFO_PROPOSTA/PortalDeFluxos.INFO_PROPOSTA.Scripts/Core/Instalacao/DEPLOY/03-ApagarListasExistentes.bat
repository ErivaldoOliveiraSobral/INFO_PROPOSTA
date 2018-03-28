@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\03-ApagarListasExistentes.ps1"
@ECHO ON
pause
