@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\11-AtualizaLembreteEmails .ps1"
@ECHO ON
pause
