using System;
using System.Collections.Generic;
using System.IO;
using Westwind.Utilities;

namespace Westwind.SetResolution
{
    public class AppConfiguration
    {
        public static AppConfiguration Current { get; set; }
        
        public List<DisplayProfile> Profiles { get; set; }  = new List<DisplayProfile>();


        public static void Load()
        {
            var file = Path.Combine(Program.StartupPath, "SetResolution.xml");

            if (File.Exists(file))
                Current = SerializationUtils.DeSerializeObject(file, typeof(AppConfiguration), false) as AppConfiguration;

            if (Current == null)
            {
                Current = new AppConfiguration();
                Current.Profiles.Add(new DisplayProfile()
                {
                    Name = "1080",
                    Width= 1920,
                    Height= 1080,
                    Frequency = 60,
                    BitSize= 32
                });
                Current.Profiles.Add(new DisplayProfile()
                {
                    Name = "4k",
                    Width = 3840 ,
                    Height = 2160,
                    Frequency = 60,
                    BitSize = 32
                });
            }
        }

        public static bool Save()
        {
            var file = Path.Combine(Program.StartupPath, "SetResolution.xml");
            try
            {
                SerializationUtils.SerializeObject(Current, file, false);
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}