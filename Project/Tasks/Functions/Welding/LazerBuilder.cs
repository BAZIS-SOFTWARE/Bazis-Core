using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Project.Tasks.FrameCreators;

namespace Project.Tasks.Functions.Welding
{
    public class LazerBuilder
    {
        Lazer lazer;

        public LazerBuilder()
        {
            lazer = new Lazer();
            lazer.Name = "LW";
            lazer.LocalFrame = new StaticFrame();
        }

        public LazerBuilder SetPower(string power)
        {
            lazer.SurfacePower = float.Parse(power, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }
        public LazerBuilder SetBottomDiam(string diam)
        {
            lazer.BottomDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public LazerBuilder SetUpperDiam(string diam)
        {
            lazer.UpperDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public LazerBuilder SetLength(string length)
        {
            lazer.Length = float.Parse(length, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public LazerBuilder SetFrame(Frame frame)
        {
            lazer.LocalFrame = new StaticFrame(frame);
            return this;
        }

        public static implicit operator Lazer(LazerBuilder builder)
        {
            return builder.lazer;
        }
    }
}
