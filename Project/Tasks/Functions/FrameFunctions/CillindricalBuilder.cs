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

namespace Project.Tasks.Functions.FrameFunctions
{
    public class CillindricalBuilder
    {
        CillindricalFunction cil;

        public CillindricalBuilder()
        {
            cil = new CillindricalFunction();
        }

        public CillindricalBuilder SetBottomDiam(string diam)
        {
            cil.BottomDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public CillindricalBuilder SetUpperDiam(string diam)
        {
            cil.UpperDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public CillindricalBuilder SetLength(string length)
        {
            cil.Length = float.Parse(length, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public static implicit operator CillindricalFunction(CillindricalBuilder builder)
        {
            return builder.cil;
        }
    }
}
