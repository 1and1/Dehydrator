@echo off
pushd "%~dp0"

if "%1" == "" set BuildConfiguration=Release
if not "%1" == "" set BuildConfiguration=%1

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS (
  echo ERROR: No Visual Studio 2015 installation found. >&2
  popd
  exit /b 1
)
call "%VS140COMNTOOLS%vsvars32.bat"

echo Compiling Visual Studio solution...
nuget restore Dehydrator.sln
msbuild Dehydrator.sln /nologo /v:q /m /t:Rebuild /p:Configuration=%BuildConfiguration%;VersionSuffix=""
if errorlevel 1 pause

echo Creating NuGet packages...
mkdir build\%BuildConfiguration%\Packages
nuget pack Dehydrator.Core\Dehydrator.Core.csproj -Verbosity quiet -Properties Configuration=%BuildConfiguration%;VersionSuffix="" -Symbols -OutputDirectory build\%BuildConfiguration%\Packages
if errorlevel 1 pause
nuget pack Dehydrator\Dehydrator.csproj -Verbosity quiet -Properties Configuration=%BuildConfiguration%;VersionSuffix="" -Symbols -OutputDirectory build\%BuildConfiguration%\Packages
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework\Dehydrator.EntityFramework.csproj -Verbosity quiet -Properties Configuration=%BuildConfiguration%;VersionSuffix="" -Symbols -OutputDirectory build\%BuildConfiguration%\Packages
if errorlevel 1 pause
nuget pack Dehydrator.EntityFramework.Unity\Dehydrator.EntityFramework.Unity.csproj -Verbosity quiet -Properties Configuration=%BuildConfiguration%;VersionSuffix="" -Symbols -OutputDirectory build\%BuildConfiguration%\Packages
if errorlevel 1 pause
nuget pack Dehydrator.WebApi\Dehydrator.WebApi.csproj -Verbosity quiet -Properties Configuration=%BuildConfiguration%;VersionSuffix="" -Symbols -OutputDirectory build\%BuildConfiguration%\Packages
if errorlevel 1 pause

popd