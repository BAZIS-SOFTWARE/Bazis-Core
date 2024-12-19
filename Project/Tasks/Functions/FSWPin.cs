using System;
using System.Collections.Generic;
using Geometry;
using Project.Interfaces.Tasks;

namespace Project.Tasks.Functions
{
    public class FSWPin : CillindricalFunction, IPowerFunction
    {      

        public float RotSpeed { get; internal set; }

        //public string YieldStressFunc { get; internal set; }

        //public float YieldStressValue { get; set; } = 1;
        /// <summary>
        /// parameters[0] = YieldStressFunc.
        /// </summary>
        List<FunctionParameter> parameters = new List<FunctionParameter>();

        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Join(";",
                Name, RotSpeed, Length, BottomDiam, UpperDiam, parameters[0].Name);
        }

/// <inheritdoc/>

        public float CalcPower(Frame frame, Point3D elementCenter)
        {
            float r, r0, r1, r2;

            //var H = float.Parse(heatSourceParam.Split(',')[3]); // light heght
            if (IsPointInside(frame, elementCenter, out r, out r0, out r1, out r2))
            {
                var pinVol = (1.0f / 3) * (float)Math.PI * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var m = 0.16f; // уточнить коэффициент рассеяния механической энергии
                var pinPower = parameters[0].Value * r * RotSpeed * m;
                return pinPower / pinVol;
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
            var fParam = new FunctionParameter() { Name = name};
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
