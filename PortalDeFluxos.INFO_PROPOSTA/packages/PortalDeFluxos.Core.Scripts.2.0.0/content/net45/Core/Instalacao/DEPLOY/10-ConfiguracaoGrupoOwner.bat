@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\10-ConfiguracaoGrupoOwner.ps1"
@ECHO ON
pause
