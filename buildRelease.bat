@echo off


set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"

set VERSIONFILE=automatedscreenshots.version
copy automatedscreenshots.version a.version
set VERSIONFILE=a.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

type automatedscreenshots.version

echo Version:  %VERSION%
del a.version


rem xcopy src\Textures\AS*.* Gamedata\AutomatedScreenShots\Textures /y
xcopy src\Textures\Auto*.png   GameData\AutomatedScreenShots\Textures /Y
copy bin\Release\AutomatedScreenshots.dll Gamedata\AutomatedScreenShots\Plugins
copy  automatedscreenshots.version Gamedata\AutomatedScreenShots
copy README.md Gamedata\AutomatedScreenshots
copy ChangeLog.txt Gamedata\AutomatedScreenshots


set FILE="%RELEASEDIR%\AutomatedScreenshots-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\AutomatedScreenshots

rem %ZIP% a -tzip %RELEASEDIR%\AutomatedScreenshots-%VERSION%.zip Gamedata\AutomatedScreenshots
