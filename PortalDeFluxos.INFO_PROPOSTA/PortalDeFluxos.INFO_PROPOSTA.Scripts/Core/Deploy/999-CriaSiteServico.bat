@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\999-CriaSiteServico.ps1"
@ECHO ON
pause
