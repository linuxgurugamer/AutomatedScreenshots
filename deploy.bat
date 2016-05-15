Rem unusable line

set H=R:\KSP_1.1.2_dev
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



xcopy bin\Debug  %H%\GameData\AutomatedScreenShots\Plugins\  /Y
xcopy src\Textures\*  %H%\GameData\AutomatedScreenShots\Textures /Y
