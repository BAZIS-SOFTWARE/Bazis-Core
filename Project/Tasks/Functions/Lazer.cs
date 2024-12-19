using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    public class Lazer : CillindricalFunction, IPowerFunction
    {
        /// <summary>
        /// SurfacePower
        /// </summary>
        public float SurfacePower { get; internal set; }

        List<FunctionParameter> parameters = new List<FunctionParameter>();
        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4}", Name, SurfacePower, Length,UpperDiam, BottomDiam);
        }
/// <inheritdoc/>

        public float CalcPower(Frame frame, Point3D elementCenter)
        {
            float r, r0, r1, r2;
            
            if (IsPointInside(frame, elementCenter, out r, out r0, out r1, out r2))
            {
                var vol = (1.0f / 3) * 3.14f * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var powerDensity = SurfacePower / vol;

                var relPowerDensity = powerDensity * (float)Math.Exp(-1 * (r * r / (r0 * r0)));
                return relPowerDensity;
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