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

            ColorConsole.WriteWrappedHeader($"Set Resolution v{ver}");
            
            if (args == null || args.Length == 0 || args[0] == "HELP" || args[0] == "/?")
            {

                Console.WriteLine("Set Display Resolution to any of the available machine display mode.");


                string options = $@"
Syntax:
-------
SetResolution  [<ProfileName>|SET|LIST|PROFILES|CREATEPROFILE]
               -w 1920 -h 1080 -f 60 -b 32 -o 0 -p ProfileName 
";

                Console.WriteLine(options);

                ColorConsole.WriteWarning(
                    "\nWarning: Setting an invalid mode may leave your screen unaccessible. Only pick supported modes.");

                options = $@"
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
-la                 List all Display modes (LIST command). Default only shows current matches

Examples:
---------
SetResolution MyProfile
SetResolution SET -p MyProfile
SetResolution SET -w 1920 -h 1080 -f 60
SetResolution LIST 
SetResolution PROFILES
SetResolution CREATEPROFILE -p ""My Profile"" -w 1920 -h 1080 -f 60
";
                Console.WriteLine(options);
            }
            else
            {
                Console.WriteLine();

                var processor = new SetResolutionProcessor(cmdLine);
                processor.Process();
            }


            AppConfiguration.Save();

#if DEBUG

            Console.WriteLine("\nDone. Press any key to exit...");
            Console.ReadKey();
#endif

        }
    }
}
