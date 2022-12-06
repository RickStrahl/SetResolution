using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using Westwind.SetResolution.CommandLine;

namespace Westwind.SetResolution
{
    public class Program
    {
        public static string StartupPath { get; set; }

        static void Main(string[] args)
        {
            StartupPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            AppConfiguration.Load();

            var cmdLine = new SetResolutionCommandLineParser();
            cmdLine.Parse();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var ver = version.Major + "." + version.Minor + (version.Build > 0 ? "." + version.Build : string.Empty);

            string text = $"Set Resolution v{ver}";
            ColorConsole.WriteLine(text, ConsoleColor.Yellow);
            ColorConsole.WriteLine(new string('-',text.Length), ConsoleColor.Yellow);
            ColorConsole.WriteLine("(c) West Wind Technologies, 2022", ConsoleColor.DarkGray);
            
            if (args == null || args.Length == 0 || args[0] == "HELP" || args[0] == "/?")
            {

                Console.WriteLine("\nSet Monitor Display Resolution to any available machine display mode.");


                string options = $@"
[cyan]Syntax[/cyan]
------
[yellow]SetResolution  <ProfileName>|SET|LIST|PROFILES|CREATEPROFILE
               -w 1920 -h 1080 -f 60 -b 32 -o 0 -p ProfileName[/yellow]

[cyan]Commands[/cyan]
--------
HELP || /?          This help display
<ProfileName>       Apply Display settings from a named Profile 
SET                 Sets Display Settings - 
                    provide either a profile (-p) or display options -w/-h/-f/-b/-o
LIST                Lists all available display modes and monitors
PROFILES            Lists all saved profiles (stored in SetResolution.xml)
CREATEPROFILE       Creates a new profile by specifying name and display options
                    - {Path.Combine(StartupPath,"SetResolution.xml")}

[cyan]Display Settings[/cyan]
----------------
-w                  Display Width
-h                  Display Height
-f                  Display Frequency in Hertz (60*)
-o                  Orientation - 0 (default*), 1 (90deg), 2 (180deg), 3 (270deg)
-p                  Profile name
-noprompt           Don't prompt for confirmation of new settings

[cyan]Command Modifiers[/cyan]
-----------------
-m                  Monitor Id  to apply command to (1,2,3 etc - use LIST to see Ids)
                    applies to: LIST, SET. If not specified, Default monitor is used.
-la                 List all Display modes (LIST command). Default only shows current matches

[cyan]Examples[/cyan]
--------
SetResolution MyProfile
SetResolution SET -p MyProfile -m2
SetResolution SET -w 1920 -h 1080 -f 60 -m2 -noprompt
SetResolution LIST -m2
SetResolution PROFILES
SetResolution CREATEPROFILE -p ""My Profile"" -w 1920 -h 1080 -f 60
";
                ColorConsole.WriteEmbeddedColorLine(options);
            }
            else
            {
                Console.WriteLine();

                var processor = new SetResolutionProcessor(cmdLine);
                processor.Process();

                Console.WriteLine();
            }


            AppConfiguration.Save();
        }
    }
}
