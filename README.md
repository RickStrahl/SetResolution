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

The help information is as follows:

```txt
Syntax:
-------
SetResolution  [SET|LIST|PROFILES|CREATEPROFILE]  -w 1920 -h 1080 -f 60 -p ProfileName

Commands:
---------
HELP || /?          This help display
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



