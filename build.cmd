@echo off
cd /d "%~dp0"

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS goto err_no_vs
call "%VS140COMNTOOLS%vsvars32.bat"

::Compile Visual Studio solution
msbuild EntityReferenceStripper.sln /nologo /t:Rebuild /p:Configuration=Release
if errorlevel 1 pause

::Create NuGet packages
FOR %%A IN ("%~dp0*.nuspec") DO (
  nuget pack "%%A" -Symbols -OutputDirectory build -Version %1
  if errorlevel 1 pause
)


goto end
rem Error messages

:err_no_vs
echo ERROR: No Visual Studio 2015 installation found. >&2
pause
goto end

:end
