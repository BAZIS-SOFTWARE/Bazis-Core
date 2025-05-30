using Geometry;
using Project.Tasks.FrameCreators;
using System.Globalization;

namespace Project.Tasks.Functions.Welding
{
    public class ArcSBuilder
    {
        ArcS arcHeatSource;

        public ArcSBuilder()
        {
            arcHeatSource = new ArcS();
            arcHeatSource.Name = "ARC";
            arcHeatSource.LocalFrame = new StaticFrame();
        }

        public ArcSBuilder SetFrame(Frame frame)
        {
            arcHeatSource.LocalFrame = new StaticFrame(frame);
            return this;
        }

        public ArcSBuilder SetCurrent(string current)
        {
            arcHeatSource.Current = float.Parse(current, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public ArcSBuilder SetWidth(string width)
        {
            arcHeatSource.Width = float.Parse(width, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public ArcSBuilder SetVoltage(string voltage)
        {
            arcHeatSource.Voltage = float.Parse(voltage, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public static implicit operator ArcS(ArcSBuilder builder)
        {
            return builder.arcHeatSource;
        }
    }
}
