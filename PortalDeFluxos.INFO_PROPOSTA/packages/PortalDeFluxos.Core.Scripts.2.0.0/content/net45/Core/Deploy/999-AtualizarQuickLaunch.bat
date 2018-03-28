@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\999-AtualizarQuickLaunch.ps1"
@ECHO ON
pause
