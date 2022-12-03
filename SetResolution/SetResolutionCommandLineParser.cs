using System;
using System.Runtime.CompilerServices;
using HtmlPackager.ConsoleApp;

namespace Westwind.SetResolution
{
    public class SetResolutionCommandLineParser : CommandLineParser
    {

        public int Width { get; set; }

        public int Height { get; set;  }

        public int Frequency { get; set; } = 60;
        
        public int BitSize { get; set; } = 32;

        public int Orientation { get; set; } = 0;

        public string Profile { get; set;  }


        public bool Help { get; set;  }
        
        public bool StartTray { get; set; }
        
        public bool CreateProfile { get; set; }
        public bool ShowResolutions { get; set; }
        

        public SetResolutionCommandLineParser(string[] args = null, string cmdLine = null)
            : base(args, cmdLine)
        {
        }

        public override void Parse()
        {
            Width = ParseIntParameterSwitch("-w", 0);
            Height = ParseIntParameterSwitch("-h", 0);
            Frequency = ParseIntParameterSwitch("-f", 60);
            BitSize = ParseIntParameterSwitch("-b", 32);
            Orientation = ParseIntParameterSwitch("-o", 0);

            Profile = ParseStringParameterSwitch("-p");

            Help = ParseParameterSwitch("-h");
        }


    }
}
