# AutomatedScreenshots (AS)

A mod to grab automated screenshots at specific time intervals and special events

Additionally, can do automated saves.

This mod will take screenshots at specified intervals.

The following are the ways that screenshots can be taken:

1.  The intervals can be specified by time in seconds between each screenshot.  
    The smallest interval allowed is one tenth of a second, so if you really want 
    to kill your performance and disk, you can specify an interval of 0.1 second
2.  Screenshots can be taken at each scene change
3.  Screenshots can be taken at special events.
4.  The UI can be hidden during the screenshots.  Be aware that this will be visible, 
    the UI will flicker off and then on during the screenshot.
5.  If options to both show and hide the UI are specified, then two screenshots will 
    be taken, one with and one without the UI
6.  Pre crash/landing detection is added, so if specified, then faster screenshots 
    will be taken just before crashes

New Feature:

7.  Integration with Historian.  If Historian is loaded and configured, AS will call 
    Historian before each screenshot to activate the ribbon

New Feature:

8.  Automatic Saves.  The mod will now do automatic saves at specified intervals.  It will 
    only keep a specified number of save files.


The screenshots are saved as PNG files.  PNG files can be big, so you can
also specify that the PNG files be converted to JPG files, and optionally
delete the PNG file after conversion.

The filename is fully configurable, by using variables in the file name.

The following are the "variables" available in file names:

[date] = Parsed DateString
[UT] = Current in-game time in seconds
[save] = Name of current save game
[vessel] = Active Vessel name
[body] = Current primary celestial body name
[situation] = Active Vessel situation (PRELAUNCH, FLYING, ORBITING, etc.)
[biome] = Current Active Vessel biome
[year] = Current in-game year (as seen in top left during flight (1, 2, etc.))
[day] = Current in-game day (similar to year)
[hour] = in-game hour
[min] = in-game minute
[sec] = in-game seconds
[evt] = event flag

Event Flag

If a screenshot by time interval: time
If by scene change: scene
If by special event: event

An example (ridiculous) possible filename is: 
AS_[date]_[save]_[vessel]_[body]_[biome]_[situation]_Y[year]_D[day]_H[hour]_M[min]_S[sec]_UT[UT] 

which could turn out to be: 
AS_2015-04-30-22-22_KSPv1.0.0_SaveGameTest_Kerbal#X_Kerbin_Shores_PRELAUNCH_Y1_D10_H5_M7_S36_UT212856.png


The code for the JPG conversion and the custom filenames was taken from the Sensible Screenshot mod, written by magico13

Automated Saves

The mod will now do automatic saves at specified intervals. it will only keep a specified number of save files.
Automatic saves is turned on by hitting Ctrl-F5