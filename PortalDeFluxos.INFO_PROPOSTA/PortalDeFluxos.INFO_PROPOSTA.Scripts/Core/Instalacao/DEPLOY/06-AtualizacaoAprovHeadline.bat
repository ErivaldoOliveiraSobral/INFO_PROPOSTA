@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\06-AtualizacaoAprovHeadline.ps1"
@ECHO ON
pause
