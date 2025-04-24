using Geometry;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Project.Tasks.Functions
{
    public class Arc : SphereFunction, IPowerFunction
    {
        public float Current { get; internal set; }
        public float Voltage { get; internal set; }
        public float Power { get { return Current * Voltage; } }

        List<FunctionParameter> parameters = new List<FunctionParameter>();
        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,"{0};{1};{2};{3}", Name, Width, Current, Voltage);
        }
/// <inheritdoc/>

        public float CalcPower(Point3D elementCenter)
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
