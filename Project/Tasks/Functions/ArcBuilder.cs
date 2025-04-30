using Geometry;
using System.Globalization;

namespace Project.Tasks.Functions
{
    public class ArcBuilder
    {
        Arc arcHeatSource;

        public ArcBuilder()
        {
            arcHeatSource = new Arc();
            arcHeatSource.Name  = "ARC";
            arcHeatSource.Frame = new Frame();
        }

        public ArcBuilder SetFrame(Frame frame)
        {
            arcHeatSource.Frame = frame;
            return this;
        }

        public ArcBuilder SetCurrent(string current)
        {
            arcHeatSource.Current = float.Parse(current,NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public ArcBuilder SetWidth(string width)
        {
            arcHeatSource.Width = float.Parse(width, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public ArcBuilder SetVoltage(string voltage)
        {
            arcHeatSource.Voltage = float.Parse(voltage, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }      

        public static implicit operator Arc(ArcBuilder builder)
        {
            return builder.arcHeatSource;
        }
    }
}
