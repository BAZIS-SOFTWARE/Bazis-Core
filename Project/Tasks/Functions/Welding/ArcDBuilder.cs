using Geometry;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Project.Tasks.Functions.Welding
{
    public class ArcDBuilder
    {
        ArcD arcHeatSource;

        public ArcDBuilder()
        {
            arcHeatSource = new ArcD();
            arcHeatSource.Name = "ARCD";
            arcHeatSource.LocalFrame = new StaticFrame();
        }

        public ArcDBuilder SetWidth(float width)
        {
            arcHeatSource.Width = width;
            return this;
        }

        public ArcDBuilder SetDepth(float depth)
        {
            arcHeatSource.Depth = depth;
            return this;
        }

        public ArcDBuilder SetFrontPart(float frontPart)
        {
            arcHeatSource.FrontPart = frontPart;
            return this;
        }

        public ArcDBuilder SetBackPart(float backPart)
        {
            arcHeatSource.BackPart = backPart;
            return this;
        }

        public ArcDBuilder SetFrame(Frame frame)
        {
            arcHeatSource.LocalFrame = new StaticFrame(frame);
            return this;
        }

        public ArcDBuilder SetCurrent(string current)
        {
            arcHeatSource.Current = float.Parse(current, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        //TO Do добавить методы инициализации параметров

        public ArcDBuilder SetVoltage(string voltage)
        {
            arcHeatSource.Voltage = float.Parse(voltage, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public static implicit operator ArcD(ArcDBuilder builder)
        {
            return builder.arcHeatSource;
        }
    }
}
