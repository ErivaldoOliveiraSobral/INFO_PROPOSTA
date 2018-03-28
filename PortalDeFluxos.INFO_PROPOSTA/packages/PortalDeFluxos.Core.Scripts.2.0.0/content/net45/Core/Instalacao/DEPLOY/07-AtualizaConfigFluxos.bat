@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\07-AtualizaConfigFluxos.ps1"
@ECHO ON
pause
