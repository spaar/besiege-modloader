@echo off

:: Make sure a version number was passed
if %1.==. goto noVersion
set RELEASE=%1

echo Creating new release: %RELEASE%

:: Create release directories
set RELEASEPATH=Releases\%RELEASE%
md %RELEASEPATH%
md %RELEASEPATH%\Normal
md %RELEASEPATH%\Developer

:: Build the project. This requires devenv to be available.
:: You can either run this file from a VS command prompt or run vsvars.bat or VsDevCmd.bat before this file.
devenv /build Release ModLoader.sln
devenv /build ReleaseDeveloper ModLoader.sln

:: Assemble normal release
copy Changelog.md %RELEASEPATH%\Normal\Changelog.txt
copy README_enduser.md %RELEASEPATH%\Normal\README.txt
copy LICENSE %RELEASEPATH%\Normal\LICENSE
copy Assembly-UnityScript.dll %RELEASEPATH%\Normal\Assembly-UnityScript.dll
copy ModLoader\bin\Release\SpaarModLoader.dll %RELEASEPATH%\Normal\SpaarModLoader.dll
md %RELEASEPATH%\Normal\Resources
md %RELEASEPATH%\Normal\Resources\ModLoader
xcopy ModLoader\Resources %RELEASEPATH%\Normal\Resources\ModLoader\ /s/h/e/k/f/c

:: Assemble developer release
md %RELEASEPATH%\Developer\DebugHelper
copy Changelog.md %RELEASEPATH%\Developer\Changelog.txt
copy README_enduser.md %RELEASEPATH%\Developer\README.txt
copy README_debughelper.md %RELEASEPATH%\Developer\DebugHelper\README_debughelper.txt
copy LICENSE %RELEASEPATH%\Developer\LICENSE
copy Assembly-UnityScript.dll %RELEASEPATH%\Developer\Assembly-UnityScript.dll
copy ModLoader\bin\ReleaseDeveloper\SpaarModLoader.dll %RELEASEPATH%\Developer\SpaarModLoader.dll
md %RELEASEPATH%\Developer\Resources
md %RELEASEPATH%\Developer\Resources\ModLoader
xcopy ModLoader\Resources %RELEASEPATH%\Developer\Resources\ModLoader\ /s/h/e/k/f/c
copy DebugHelper\bin\Release\DebugHelper.exe %RELEASEPATH%\Developer\DebugHelper\DebugHelper.exe
copy DebugHelper\bin\Release\Mono.Cecil.dll %RELEASEPATH%\Developer\DebugHelper\Mono.Cecil.dll
copy DebugHelper\bin\Release\Mono.Cecil.Mdb.dll %RELEASEPATH%\Developer\DebugHelper\Mono.Cecil.Mdb.dll
copy DebugHelper\bin\Release\Mono.Cecil.Pdb.dll %RELEASEPATH%\Developer\DebugHelper\Mono.Cecil.Pdb.dll
copy DebugHelper\bin\Release\Mono.Cecil.Rocks.dll %RELEASEPATH%\Developer\DebugHelper\Mono.Cecil.Rocks.dll

goto :end

:noVersion
echo No version number!
goto :end

:end