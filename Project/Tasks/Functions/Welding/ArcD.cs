using Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Project.Tasks.Functions.Welding
{
    public class ArcD : DoubleEllips
    {
        public float Current { get; set; }
        public float Voltage { get; set; }
        public float Power { get { return Current * Voltage; } }


        /// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name};{Width};{Current};{Voltage};{LocalFrame}";
        }
        /// <inheritdoc/>

        public override float CalcValue(Point3D elementCenter)
        {

            if (IsPointInside(elementCenter))
            {
                /*
                var power = Power * 0.85f;
                var vol = halfBead * halfBead * halfBead;
                var powerDensity = power / vol;

                return powerDensity * (float)Math.Exp(-1 * (rad * rad / (halfBead * halfBead)));
                */
            }
            return 0;
        }
    }
}
