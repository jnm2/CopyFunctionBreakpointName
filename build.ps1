$ErrorActionPreference = "Stop";

. 'build\Update-Version.ps1'
Update-Version

$visualStudioInstallation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
$configuration = 'Release'
$targetFramework = 'net472'

$msbuild = Join-Path $visualStudioInstallation 'MSBuild\15.0\Bin\MSBuild.exe'
& $msbuild src /restore /p:Configuration=$configuration /v:minimal

$vstest = join-path $visualStudioInstallation 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\VSTest.Console.exe'
& $vstest src\CopyFunctionBreakpointName.Tests\bin\$configuration\$targetFramework\CopyFunctionBreakpointName.Tests.dll
