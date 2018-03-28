@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\09-ConfiguracaoSessionAppFrabic.ps1"
@ECHO ON
pause
