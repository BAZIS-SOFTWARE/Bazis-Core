using Geometry;
using System;
using System.Globalization;

namespace Project.Tasks.Functions
{
    public class FSWShoulderBuilder
    {
        FSWShoulder shoulder;

        public FSWShoulderBuilder()
        {
            shoulder = new FSWShoulder();
            shoulder.Name = "FSWShoulder";
            shoulder.Frame = new Frame();
        }

        public FSWShoulderBuilder SetRotSpeed(string rotSpeed)
        {
            shoulder.RotSpeed = float.Parse(rotSpeed, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWShoulderBuilder SetAxisForce(string axisForce)
        {
            shoulder.AxisForce = float.Parse(axisForce, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWShoulderBuilder SetFricModuleFunc(string fricModuleFunc)
        {
            shoulder.CreateParameter(fricModuleFunc);
            return this;
        }

        public FSWShoulderBuilder SetBottomDiam(string diam)
        {
            shoulder.BottomDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWShoulderBuilder SetUpperDiam(string diam)
        {
            shoulder.UpperDiam = float.Parse(diam, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWShoulderBuilder SetLength(string length)
        {
            shoulder.Length = float.Parse(length, NumberStyles.Float, CultureInfo.InvariantCulture);
            return this;
        }

        public FSWShoulderBuilder SetFrame(Frame frame)
        {
            shoulder.Frame = frame;
            return this;
        }

        public static implicit operator FSWShoulder(FSWShoulderBuilder builder)
        {
            return builder.shoulder;
        }
    }
}
