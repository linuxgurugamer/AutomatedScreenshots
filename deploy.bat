Rem unusable line

set H=R:\KSP_1.3.0_dev
echo %H%

set d=%H%
if exist %d% goto one
mkdir %d%
:one
set d=%H%\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%H%\Gamedata\AutomatedScreenShots
if exist %d% goto three
mkdir %d%
:three
set d=%H%\Gamedata\AutomatedScreenShots\Plugins
if exist %d% goto four
mkdir %d%
:four
set d=%H%\Gamedata\AutomatedScreenShots\Textures
if exist %d% goto five
mkdir %d%
:five
set d=%H%\Gamedata\AutomatedScreenShots\PluginData
if exist %d% goto six
mkdir %d%
:six



copy bin\Debug  %H%\GameData\AutomatedScreenShots\Plugins\AutomatedScreenshots.dll  /Y
xcopy src\Textures\*  %H%\GameData\AutomatedScreenShots\Textures /Y
