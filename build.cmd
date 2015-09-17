@echo off
cd /d "%~dp0"
set version=0.1.0.0

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS goto err_no_vs
call "%VS140COMNTOOLS%vsvars32.bat"

::Compile Visual Studio solution
nuget restore Dehydrator.sln
msbuild Dehydrator.sln /nologo /t:Rebuild /p:Configuration=Release
if errorlevel 1 pause

::Create NuGet packages
mkdir build\NuGet
nuget pack Dehydrator\Dehydrator.nuspec -Symbols -OutputDirectory build\NuGet -Version %version%
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework\Dehydrator.EntityFramework.nuspec -Symbols -OutputDirectory build\NuGet -Version %version%
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework.Unity\Dehydrator.EntityFramework.Unity.nuspec -Symbols -OutputDirectory build\NuGet -Version %version%
if errorlevel 1 pause
nuget pack Dehydrator.WebApi\Dehydrator.WebApi.nuspec -Symbols -OutputDirectory build\NuGet -Version %version%
if errorlevel 1 pause

goto end
rem Error messages

:err_no_vs
echo ERROR: No Visual Studio 2015 installation found. >&2
pause
goto end

:end
