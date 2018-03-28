@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\04-CriarGruposRoles.ps1"
@ECHO ON
pause
