using Geometry;
using Project.Tasks.FrameCreators;
using System.Globalization;
using static IronPython.Modules._ast;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class SphereBuilder
    {
        SphereFunction sph;

        public SphereBuilder()
        {
            sph = new SphereFunction();
        }

        public SphereBuilder SetWidth(string width)
        {
            sph.Width = float.Parse(width, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public static implicit operator SphereFunction(SphereBuilder builder)
        {
            return builder.sph;
        }
    }
}
