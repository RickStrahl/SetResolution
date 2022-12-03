using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Westwind.SetResolution.CommandLine;

namespace Westwind.SetResolution
{
    public class SetResolutionProcessor
    {
        private SetResolutionCommandLineParser CommandLine { get; }

        public SetResolutionProcessor(SetResolutionCommandLineParser commandLine)
        {
            CommandLine= commandLine;
        }

        public void Process()
        {
            if (CommandLine.FirstParameter.Equals("Set", StringComparison.OrdinalIgnoreCase))
            {
                SetResolution();
            }

            if (CommandLine.FirstParameter.Equals("List", StringComparison.OrdinalIgnoreCase))
            {
                ListDisplayModes();
            }

            if (CommandLine.FirstParameter.Equals("Profiles", StringComparison.OrdinalIgnoreCase))
            {
                ListProfiles();
            }
            
            if (CommandLine.FirstParameter.Equals("CreateProfile", StringComparison.OrdinalIgnoreCase))
            {
                CreateProfile();
            }
        }

        private void CreateProfile()
        {
            if (string.IsNullOrEmpty(CommandLine.Profile))
            {
                ColorConsole.WriteError("You have to specify a profile to create.");
                return;
            }

            if (CommandLine.Width == 0 || CommandLine.Height == 0)
            {
                ColorConsole.WriteError("Please specify at minimum a Width and Height for the profile.");
                return;
            }

            var profile = new DisplayProfile()
            {
                Name = CommandLine.Profile,
                Width = CommandLine.Width,
                Height = CommandLine.Height,
                Frequency = CommandLine.Frequency,
                BitSize = CommandLine.BitSize,
                Orientation = CommandLine.Orientation,
            };

            var set = AppConfiguration.Current.Profiles.FirstOrDefault(p =>
                p.Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase));
            if (set != null)
                AppConfiguration.Current.Profiles.Remove(set);

            AppConfiguration.Current.Profiles.Add(profile);
            AppConfiguration.Save();

            ColorConsole.WriteSuccess($"Created new Profile: {profile.Name}. {profile}");

            Console.WriteLine();

            ListProfiles();
        }

        private void SetResolution()
        {
            if (!string.IsNullOrEmpty(CommandLine.Profile))
            {
                var profile = AppConfiguration.Current.Profiles.FirstOrDefault(p=> p.Name.Equals(CommandLine.Profile, StringComparison.OrdinalIgnoreCase));
                if (profile == null)
                {
                    ColorConsole.WriteError($"Couldn't find Display Profile {CommandLine.Profile}");
                    Console.WriteLine();
                    ListProfiles();
                    return;
                }

                profile.UpdateCommandLine(CommandLine);
            }

            if (CommandLine.Width == 0 || CommandLine.Height == 0)
            {
                ColorConsole.WriteError("Please specify at minimum Width and Height parameters.");
                return;
            }

            if (CommandLine.Frequency == 0)
            {
                CommandLine.Frequency = 60;
            }

            var list = DisplayManager.GetAllDisplayModes();
            var set = list.FirstOrDefault(d=> d.Width == CommandLine.Width && 
                                    d.Height == CommandLine.Height && 
                                    d.Frequency == CommandLine.Frequency &&
                                    d.BitCount == CommandLine.BitSize &&
                                    d.Orientation == (Orientation) CommandLine.Orientation );

            if (set == null)
            {
                ColorConsole.WriteError($"Couldn't find matching Display Mode.");
                Console.WriteLine();
                ListDisplayModes();
                return;
            }

            DisplayManager.SetDisplaySettings(set);
            ColorConsole.WriteSuccess("Switched Display Mode to " + set.ToString());
        }

        private void ListDisplayModes()
        {
            var list = DisplayManager.GetAllDisplayModes();

            ColorConsole.WriteLine("Available Display Modes", ConsoleColor.Yellow);
            ColorConsole.WriteLine("-----------------------", ConsoleColor.Yellow);


            foreach (var set in list)
            {
                Console.WriteLine(set.ToString());
            }
        }

        private void ListProfiles()
        {
            var list = DisplayManager.GetAllDisplayModes();

            ColorConsole.WriteLine("Available Profiles", ConsoleColor.Yellow);
            ColorConsole.WriteLine("-----------------------", ConsoleColor.Yellow);


            foreach (var profile in AppConfiguration.Current.Profiles)
            {
                Console.WriteLine(profile.ToString());
            }
        }
    }
}
