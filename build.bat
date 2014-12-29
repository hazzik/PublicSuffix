set EnableNuGetPackageRestore=true 
set msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild
mkdir release
%msbuild% /p:Configuration=Release /p:BuildPackage=True /p:PackageOutputDir=%~dp0release

