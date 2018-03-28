@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\06-CriarSiteAnexosPropostas.ps1"
@ECHO ON
pause
