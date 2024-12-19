using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class FSWShoulderBuilder
    {
        FSWShoulder shoulder;

        public FSWShoulderBuilder()
        {
            shoulder = new FSWShoulder();
            shoulder.Name = "FSWShoulder";
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

        public static implicit operator FSWShoulder(FSWShoulderBuilder builder)
        {
            return builder.shoulder;
        }
    }
}
