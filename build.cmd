@echo off
cd /d "%~dp0"

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS goto err_no_vs
call "%VS140COMNTOOLS%vsvars32.bat"

::Compile Visual Studio solution
nuget restore Dehydrator.sln
msbuild Dehydrator.sln /nologo /t:Rebuild /p:Configuration=Release
if errorlevel 1 pause

::Create NuGet packages
mkdir build\Packages
nuget pack Dehydrator.Core\Dehydrator.Core.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack Dehydrator\Dehydrator.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework\Dehydrator.EntityFramework.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework.Unity\Dehydrator.EntityFramework.Unity.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack Dehydrator.WebApi\Dehydrator.WebApi.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause

goto end
rem Error messages

:err_no_vs
echo ERROR: No Visual Studio 2015 installation found. >&2
pause
goto end

:end
