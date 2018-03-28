@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\999-AplicarGruposRolesPaginas.ps1"
@ECHO ON
pause
