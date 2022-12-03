using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Westwind.SetResolution
{
    public class DisplayProfile
    {

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Frequency { get; set; } = 60;

        public int BitSize { get; set; } = 32;

        public int Orientation { get; set; } = 0;

        public void UpdateCommandLine(SetResolutionCommandLineParser cmd)
        {

            cmd.Width = Width;
            cmd.Height = Height;
            cmd.Frequency = Frequency;
            cmd.BitSize = BitSize;
            cmd.Orientation = Orientation;

        }

        public override string ToString()
        {
            return $"{Name}:  {Width} x {Height}, {BitSize}, {Frequency}";
        }

    }
}
