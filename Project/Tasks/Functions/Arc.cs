using Geometry;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Project.Tasks.Functions
{
    public class Arc : SphereFunction
    {

        public float Current { get; set; }
        public float Voltage { get; set; }
        public float Power { get { return Current * Voltage; } }


        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,"{0};{1};{2};{3}", Name, Width, Current, Voltage);
        }
/// <inheritdoc/>

        public override float CalcValue(Point3D elementCenter)
        {
            float rad, halfBead;

            if (IsPointInside(elementCenter, out rad, out halfBead))
            {

                var power = Power * 0.85f;
                var vol = halfBead * halfBead * halfBead;
                var powerDensity = power / vol;

                return powerDensity * (float)Math.Exp(-1 * ((rad * rad) / (halfBead * halfBead)));
            }
            return 0;
        }       
    }
}
