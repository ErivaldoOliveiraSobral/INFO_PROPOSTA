@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\08-ConfiguracaoPdf.ps1"
@ECHO ON
pause
