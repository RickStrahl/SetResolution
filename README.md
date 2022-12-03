# Set Windows Display Resolution from Command Line

This small command line utility allows you to quickly set Windows Display Resolutions. 

* Set Display Resolution
* List all available Display Modes
* Create multiple Profiles for quick access

## Download
This tool is a small, self-contained .NET Console EXE application and you can download that `SetResolution.exe` file directly from here:

[Download SetResolution.exe](https://github.com/RickStrahl/SetResolution/raw/master/Binaries/SetResolution.exe)


## Syntax:
To show available syntax, run the program without any parameters or `/?`. 

> **Warning:** Use at your own risk. Setting an invalid display mode can leave your screen inaccessible. Use only with supported display modes.

The help information is as follows:

```txt
Syntax:
-------
SetResolution  [<ProfileName>|SET|LIST|PROFILES|CREATEPROFILE]
               -w 1920 -h 1080 -f 60 -b 32 -o 0 -p ProfileName

Commands:
---------
HELP || /?          This help display
<ProfileName>       Run with only a Profile Name sets that display profile
SET                 Sets Display Settings -
                    provide either a profile (-p) or display options -w/-h/-f/-b/-o
LIST                Lists all available display modes
PROFILES            Lists all saved profiles (stored in SetResolution.xml)
CREATEPROFILE       Creates a new profile by specifying name and display options

Display Settings:
-----------------
-w                  Display Width
-h                  Display Height
-f                  Display Frequency in Hertz (60*)
-o                  Orientation - 0 (default*), 1 (90deg), 2 (180deg), 3 (270deg)
-p                  Profile name

Examples:
---------
SetResolution MyProfile
SetResolution SET -p 1080    (a profile name)
SetResolution SET -w 1920 -h 1080 -f 60
SetResolution LIST
SetResolution PROFILES
SetResolution CREATEPROFILE -p "My Profile" -w 1920 -h 1080 -f 60
```

### Add to the Windows Path
We recommend that you add the `SetResolution.exe` folder to your Windows path so that you can always and quickly access the application to switch resolution from anywhere.

### Profiles
Profiles are 'shortcuts' to a specific set of Display Settings with a name and you can quickly access a profile with:

```ps
SetResolution SET -p <profileName>
```

You can create a profile with:

```ps
SetResolution CREATEPROFILE -p <profileName> -w 1280 -h 768 -f 59
```

Profiles are stored in `SetResolution.xml` in the same folder as the .exe. To remove profiles you can edit the `SetResolution.xml` file.

## Fark: I Set a Resolution that doesn't work. Now what?
If you accidentally set your monitor into a display mode that isn't supported or just doesn't work, it's possible that the your screen becomes inaccessible. Because this tool switches the default display settings, once a wrong setting is made the screen simply will be blank and it's not just a simple matter of rebooting as the setting is applied to the Windows settings and persists on a reboot.

To reset an invalid setting you have to **boot into Windows Safe Mode** and select another display adapter, then reboot.

> Moral of the Story: Pick a display mode that you know works!

Note it's not easy to do this - as we set the display mode only to modes that have been retrieved just before - so realistically display modes should never be mismatched to what the monitor supports. Still it's possible of the hundreds of display modes available for many adapters that some may not work.

## Credits
The hard work of this tool is in the Win32 interfaces to retrieve and set display settings. All of that code is based on this article on C# Corner by [Mohammad Elseheimy](https://www.c-sharpcorner.com/members/mohammad-elsheimy):

* [Changing Display Settings Programmatically
](https://www.c-sharpcorner.com/uploadfile/GemingLeader/changing-display-settings-programmatically/)


