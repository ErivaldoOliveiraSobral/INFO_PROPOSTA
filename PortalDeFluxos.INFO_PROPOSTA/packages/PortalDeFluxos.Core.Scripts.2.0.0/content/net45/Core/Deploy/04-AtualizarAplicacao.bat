@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\04-AtualizarAplicacao.ps1"
@ECHO ON
pause
