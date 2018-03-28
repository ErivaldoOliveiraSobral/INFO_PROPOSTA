@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\05-ImportarRolesLista.ps1"
@ECHO ON
pause
