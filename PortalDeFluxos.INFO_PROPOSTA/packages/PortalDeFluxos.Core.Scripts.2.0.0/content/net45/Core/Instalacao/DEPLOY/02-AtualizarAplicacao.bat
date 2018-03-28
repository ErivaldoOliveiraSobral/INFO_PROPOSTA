@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\02-AtualizarAplicacao.ps1"
@ECHO ON
pause
