using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            ListProfiles();
        }

        private void SetResolution()
        {
            var devices = DisplayManager.GetAllDisplayDevices();
            var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
            var currentSettings = DisplayManager.GetCurrentDisplaySetting();

            if (CommandLine.MonitorId > 0)
            {
                monitor = devices.FirstOrDefault(d => d.Index == CommandLine.MonitorId);
                devices.ForEach(d=> d.IsSelected = false);
                monitor.IsSelected = true;
            }


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

            var list = DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);

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

            set.NoPersist = CommandLine.NoPersist;

            try
            {
                DisplayManager.SetDisplaySettings(set, monitor.DriverDeviceName);
                ColorConsole.WriteEmbeddedColorLine($"Switched Display Mode on Monitor [green]{monitor.DisplayName}[/green] to:\n[green]{set.ToString()}[/green]");
            }
            catch(Exception ex)
            {
                ColorConsole.WriteError("Unable to set Display Mode to " + set.ToString() + "\nError: " + ex.Message);
                return;
            }

            if (!CommandLine.NoPrompt)
            {
                ColorConsole.WriteLine("\npress any key to confirm new resolution within 5 seconds.",ConsoleColor.Yellow);
                bool keyPressed = false;
                for (int i = 0; i < 55; i++)
                {
                    Thread.Sleep(100);
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        keyPressed = true;
                        break;
                    }
                }

                if (!keyPressed)
                {
                    ColorConsole.WriteWarning("No key pressed: Resetting Display Mode to previous settings.");
                    DisplayManager.SetDisplaySettings(currentSettings, deviceName: monitor.DriverDeviceName);
                }
                else
                {
                    ColorConsole.WriteSuccess("Successfully changed resolution.");
                }
            }
        }

        private void ListDisplayModes(bool showAll = false)
        {
            string text;
            var devices = DisplayManager.GetAllDisplayDevices();
            var monitor = devices.FirstOrDefault(d => d.IsSelected);  // main monitor
            if (CommandLine.MonitorId > 0)
            {
                monitor = devices.FirstOrDefault(d => d.Index == CommandLine.MonitorId);
                devices.ForEach(d => d.IsSelected = false);
                monitor.IsSelected = true;
            }

            var displayModes = DisplayManager.GetAllDisplaySettings(monitor.DriverDeviceName);
            var current = DisplayManager.GetCurrentDisplaySetting(monitor.DriverDeviceName);

            ColorConsole.WriteLine("Available Monitors", ConsoleColor.Yellow);
            ColorConsole.WriteLine("------------------", ConsoleColor.Yellow);
            foreach (var device in devices)
            {
                if (device.IsSelected)
                    ColorConsole.WriteLine(device.ToString(), ConsoleColor.Green);
                else
                    Console.WriteLine(device);
            }
            Console.WriteLine();

            
            IList<DisplaySettings> filtered = displayModes;
            if (!CommandLine.ListAll)
            {
                filtered = displayModes.Where(d =>
                        d.Width >= AppConfiguration.Current.MinResolutionWidth &&
                        d.Frequency == current.Frequency &&
                        d.Orientation == current.Orientation)
                    .OrderByDescending(d=> d.Width)
                    // unique
                    .GroupBy(d => new {d.Width, d.Height, d.Frequency, d.Orientation})
                    .Select(g => g.First())
                    .ToList();
            }
            else
            {
                filtered = filtered.OrderByDescending(d => d.Width).ToList();
            }

            text = $"Available Display Modes ({filtered.Count})" ;
            ColorConsole.WriteLine(text, ConsoleColor.Yellow);
            ColorConsole.WriteLine(new string('-', text.Length), ConsoleColor.Yellow);


            foreach (var set in filtered)
            {
                if (set.Equals(current))
                    ColorConsole.WriteLine(set.ToString(!CommandLine.ListAll) + " *", ConsoleColor.Green);
                else
                    Console.WriteLine(set.ToString(!CommandLine.ListAll));
            }
        }

        private void ListProfiles()
        {
            var list = DisplayManager.GetAllDisplaySettings();

            ColorConsole.WriteLine("Available Profiles", ConsoleColor.Yellow);
            ColorConsole.WriteLine("-----------------------", ConsoleColor.Yellow);


            foreach (var profile in AppConfiguration.Current.Profiles)
            {
                Console.WriteLine(profile.ToString());
            }
        }
    }
}
