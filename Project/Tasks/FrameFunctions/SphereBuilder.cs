using Geometry;
using Project.Tasks.FrameCreators;
using System.Globalization;

namespace Project.Tasks.FrameFunctions
{
    public class SphereBuilder
    {
        SphereFunction sph;

        public SphereBuilder()
        {
            sph = new SphereFunction();
        }

        public SphereBuilder SetFrame(Frame frame)
        {
            sph.LocalFrame = new StaticFrame(frame);
            return this;
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
