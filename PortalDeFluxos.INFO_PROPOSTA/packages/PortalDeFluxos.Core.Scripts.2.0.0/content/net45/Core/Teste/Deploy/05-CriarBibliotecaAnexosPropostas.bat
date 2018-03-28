@ECHO OFF
cd %~dp0
powershell -ExecutionPolicy UnRestricted -File ".\05-CriarBibliotecaAnexosPropostas.ps1"
@ECHO ON
pause
