using System;
using System.Collections.Generic;
using Geometry;
using Project.Interfaces.Tasks;

namespace Project.Tasks.Functions.Welding
{
    public class FSWPin : CillindricalFunction
    {
        /// <summary>
        /// RotSpeed
        /// </summary>
        public float RotSpeed { get; set; }

        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Join(";",
                Name, RotSpeed, Length, BottomDiam, UpperDiam, parameters[0].Name);
        }

        /// <inheritdoc/>

        public override float CalcValue(Point3D elementCenter)
        {
            float r, r0, r1, r2;

            //var H = float.Parse(heatSourceParam.Split(',')[3]); // light heght
            if (IsPointInside(elementCenter, out r, out r0, out r1, out r2))
            {
                var pinVol = 1.0f / 3 * (float)Math.PI * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var m = 0.16f; // уточнить коэффициент рассеяния механической энергии
                var pinPower = parameters[0].Value * r * RotSpeed * m;
                return pinPower / pinVol;
            }
            return 0;
        }

    }
}
