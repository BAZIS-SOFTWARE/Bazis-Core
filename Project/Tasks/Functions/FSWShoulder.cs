using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Geometry;
using Project.Interfaces.Tasks;

namespace Project.Tasks.Functions
{
    public class FSWShoulder : CillindricalFunction
    {
        public float RotSpeed { get; set; }
        public float AxisForce { get; set; }
/// <inheritdoc/>

        public override string ToString()
        {
            return string.Join(";", 
                Name, AxisForce, RotSpeed,Length, BottomDiam, UpperDiam,
                parameters[0].Name);
        }
 /// <inheritdoc/>
 

        public override float CalcValue(Point3D elementCenter)
        {
            float r, r0, r1, r2;

            if (IsPointInside(elementCenter, out r, out r0, out r1, out r2))
            {
                var r3 = BottomDiam / 3; // shoulderLength;
                var frictionForce = AxisForce * parameters[0].Value; // ;
                var shearStress = frictionForce / (float)Math.PI * (r2 * r2 - r3 * r3);
       
                var sholderVol = (1.0f / 3) * (float)Math.PI * Math.Abs(Frame.Centre._z) * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var sholderPower = shearStress * r * RotSpeed;
                return sholderPower / sholderVol;
            }
            return 0;
        }       
    }
}
