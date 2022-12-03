using System;
using System.Collections.Generic;
using System.Linq;
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
                ListDisplayModes(CommandLine.ListAll);
            }

            if (CommandLine.FirstParameter.Equals("Profiles", StringComparison.OrdinalIgnoreCase))
            {
                ListProfiles();
            }
            
            if (CommandLine.FirstParameter.Equals("CreateProfile", StringComparison.OrdinalIgnoreCase))
            {
                CreateProfile();
            }

            // No Action command (help is handled on startup)
            if (CommandLine.FirstParameter.StartsWith("-"))
            {
                // assume we're using SetResolution with parameters
                SetResolution();
            }

            // Launch Profile by name
            if (AppConfiguration.Current.Profiles.Any(pro => pro.Name.Equals(CommandLine.FirstParameter, StringComparison.OrdinalIgnoreCase)))
            {
                CommandLine.Profile = CommandLine.FirstParameter;
                SetResolution();
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
                BitSize = CommandLine.BitCount,
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
                                    d.BitCount == CommandLine.BitCount &&
                                    d.Orientation == (Orientation) CommandLine.Orientation );

            if (set == null)
            {
                ColorConsole.WriteError($"Couldn't find a matching Display Mode.");
                Console.WriteLine();
                ListDisplayModes();
                return;
            }

            DisplayManager.SetDisplaySettings(set);
            ColorConsole.WriteSuccess("Switched Display Mode to " + set.ToString());
        }

        private void ListDisplayModes(bool showAll = false)
        {
            var list = DisplayManager.GetAllDisplayModes();
            var current = DisplayManager.GetCurrentDisplaySetting();


            ColorConsole.WriteLine("Current Display Mode", ConsoleColor.Yellow);
            Console.WriteLine(current + "\n");


            ColorConsole.WriteLine("Available Display Modes", ConsoleColor.Yellow);
            ColorConsole.WriteLine("-----------------------", ConsoleColor.Yellow);

            IEnumerable<DisplaySettings> filtered = list;
            if (!CommandLine.ListAll)
            {
                filtered = list.Where(d =>
                        d.Width >= 800 &&
                        d.Frequency == current.Frequency &&
                        d.Orientation == current.Orientation)
                    // unique
                    .GroupBy(d => new {d.Width, d.Height, d.Frequency, d.Orientation})
                    .Select(g => g.First());
            }

            foreach (var set in filtered)
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
