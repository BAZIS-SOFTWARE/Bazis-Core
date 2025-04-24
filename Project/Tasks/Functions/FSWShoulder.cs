using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Geometry;
using Project.Interfaces.Tasks;

namespace Project.Tasks.Functions
{
    public class FSWShoulder : CillindricalFunction, IPowerFunction
    {
        public float RotSpeed { get; internal set; }
        public float AxisForce { get; internal set; }
/// <inheritdoc/>

        //public string FrictionModuleFunc { get; internal set; }
        /// <inheritdoc/>
        //public float FrictionModulefVal { get; set; }

        /// <summary>
        /// parameters[0] = FrictionModule.
        /// </summary>
        List<FunctionParameter> parameters = new List<FunctionParameter>();

        /// <inheritdoc/>


        public override string ToString()
        {
            return string.Join(";", 
                Name, AxisForce, RotSpeed,Length, BottomDiam, UpperDiam,
                parameters[0].Name);
        }
 /// <inheritdoc/>
 

        public float CalcPower(Point3D elementCenter)
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

        /// <inheritdoc/>
        public void CreateParameter(string name, float value)
        {
            var fParam = new FunctionParameter() { Name = name, Value = value };
            parameters.Add(fParam);
        }
        /// <inheritdoc/>

        public void CreateParameter(string name)
        {
            var fParam = new FunctionParameter() { Name = name };
            parameters.Add(fParam);
        }
        /// <inheritdoc/>

        public IEnumerable<FunctionParameter> GetParameters()
        {
            foreach (var item in parameters)
            {
                yield return item;
            }
        }
    }
}
