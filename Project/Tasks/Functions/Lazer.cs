using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    public class Lazer : CillindricalFunction
    {
        /// <summary>
        /// SurfacePower
        /// </summary>
        public float SurfacePower { get; internal set; }

        /// <inheritdoc/>

        public override string ToString()
        {
            return $"{base.ToString()};{SurfacePower};{Length};{UpperDiam};{BottomDiam}";
        }
/// <inheritdoc/>

        public override float CalcPower(Point3D elementCenter)
        {
            float r, r0, r1, r2;
            
            if (IsPointInside(elementCenter, out r, out r0, out r1, out r2))
            {
                var vol = (1.0f / 3) * 3.14f * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var powerDensity = SurfacePower / vol;

                var relPowerDensity = powerDensity * (float)Math.Exp(-1 * (r * r / (r0 * r0)));
                return relPowerDensity;
            }
            return 0;
        }       
    }
}