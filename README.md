# Set Windows Display Resolution Command Line Tool

This small command line utility allows you to quickly set Windows Display Resolutions. 

* Set Display Resolution
* List all available Display Modes
* Create multiple Profiles for quick access

## Download
This tool is a small, self-contained .NET Console EXE application and you can download that `SetResolution.exe` file directly from here:


## Syntax:

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

