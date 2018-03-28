@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\06-CriarBibliotecaAnexosPropostas.ps1"
@ECHO ON
pause
