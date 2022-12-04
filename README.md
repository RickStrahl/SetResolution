# Set Windows Display Resolution from Command Line

This small command line utility allows you to quickly set Windows Display Resolutions to any of the available display modes for your video driver on the default Windows Screen device.

* Set explicit Display Resolution (has to match a valid display mode)
* List all available Display Modes
* Create and use multiple Display Mode Profiles for quick access


> **Warning:** Use at your own risk. Setting an invalid display mode can [leave your screen inaccessible](#fark-i-set-a-resolution-that-doesnt-work-now-what). Use only with supported display modes. We check your settings against available modes and only allow those that match a driver display mode, but there may still be some modes that don't work.

## Download
This tool is a small, self-contained Console EXE application and you can download the `SetResolution.exe` (or `sr.exe` file directly from here):

[Download SetResolution.exe](https://github.com/RickStrahl/SetResolution/raw/master/Binaries/SetResolution.exe)

I recommend you copy to a folder location that is in your Windows path or add it to your path, so you can run `SetResoltion` from any location.

Most common usage is with a pre-define profile name:

```powershell
# Set to a profile named 1080
SetResolution 1080

# or shortcut version (sr.exe)
sr 4k
```
## Syntax
To show available syntax, run `SetResolution.exe` or `sr.exe` without any parameters or `/?` or `HELP`. 
The help information is as follows:

```text
Syntax:
-------
SetResolution  [<ProfileName>|SET|LIST|PROFILES|CREATEPROFILE]
               -w 1920 -h 1080 -f 60 -b 32 -o 0 -la -p ProfileName

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
-la                 List all Display modes (LIST). Default only shows current matches

Examples:
---------
SetResolution MyProfile
SetResolution SET -p MyProfile
SetResolution SET -w 1920 -h 1080 -f 60
SetResolution LIST
SetResolution PROFILES
SetResolution CREATEPROFILE -p "My Profile" -w 1920 -h 1080 -f 60
```

### Add to the Windows Path
We recommend that you add the `SetResolution.exe` folder to your Windows path so that you can always and quickly access the application to switch resolution from anywhere.

### Profiles
Profiles are 'shortcuts' to a specific set of Display Settings with a name and you can quickly access a profile with:

```powershell
SetResolution SET -p <profileName>
```

You can create a profile with:

```powershell
SetResolution CREATEPROFILE -p <profileName> -w 1280 -h 768 -f 59
```

Profiles are stored in `SetResolution.xml` in the same folder as the .exe. To remove profiles you can edit the `SetResolution.xml` file.

You can also list available Profiles via the `LIST` command:

```powershell
SetResolution LIST
```

This shows a list of display modes available. By default the list only shows:

* Sizes with the Width > 800 pixels
* Frequencies that match the current display frequency
* Orientation that match the current orientation

If you want to see `all display modes` use the `-la` command line switch:

```powershell
SetResolution LIST -la
```

This displays all displays modes for all sizes, orientations and frequencies.

Use these display modes when you create new Profiles and ensure they match one of the listed modes.

#### Default Profiles
A number of default profiles are added for common 16:9 resolutions @ 60hz which is most common:

```text
Available Profiles
------------------
1080:  1920 x 1080, 32, 60
4k:  3840 x 2160, 32, 60
1440:  2560 x 1440, 32, 60
720:  1280 x 720, 32, 60
```

These are loaded on first load of the application and stored in the saved profile file (if writable).

#### Profile Location
Profiles are stored on disk in `SetResolution.xml` in the same folder as the `.exe` and you can add and remove additional profiles there or add via the `CREATEPROFILE` action as described above.

> **Note:** If you installed the EXE in a location that has no write access, saving of new Profile entries with `CREATEPROFILE` will fail silently. Either give `SetResolution.xml` read/write access or move the application to a location where you are allowed to write files.


## Fark: I set a Resolution that doesn't work. Now what?
If you accidentally set your monitor into a display mode that isn't supported or just doesn't work with your monitor, it's possible that your screen becomes inaccessible. Because this tool switches the default display settings, once a wrong setting is made the screen simply will be blank and it's not just a simple matter of rebooting as the setting is applied to the Windows settings and persists on a reboot.

To reset a non-working display setting you have to **boot into Windows Safe Mode** and select another display mode, then reboot. 

It should be very hard to do this with this tool:  We set the display mode only to modes that are available on the active video driver, so setting a non-supported resolution should never happen. However, you can end up with a resolution that your driver supports but that your monitor can't display - this mostly involves unsupported frequencies.

> Moral of the Story: Pick a display mode that you know works using common, widely used resolutions.

## Credits
Most of the hard work of this tool is in the Win32 interfaces to retrieve and set display settings. All of that code is based on this excellent article on C# Corner by [Mohammad Elseheimy](https://www.c-sharpcorner.com/members/mohammad-elsheimy):

* [Changing Display Settings Programmatically
](https://www.c-sharpcorner.com/uploadfile/GemingLeader/changing-display-settings-programmatically/)


