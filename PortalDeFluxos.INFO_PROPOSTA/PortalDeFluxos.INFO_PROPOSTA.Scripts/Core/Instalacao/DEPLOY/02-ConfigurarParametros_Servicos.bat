@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\02-ConfigurarParametros_Servicos.ps1"
@ECHO ON
pause
