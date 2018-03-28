@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\12-ConfiguracaoAmbiente.ps1"
@ECHO ON
pause
