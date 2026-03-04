using System;
using System.Collections.Generic;
using Geometry;
using IronPython.Runtime.Types;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class SphereFunction : FrameFunction
    {
        public SphereFunction()
        {
            Name = FuncName.SPH.ToString();
            FunctionType = FuncType.CPF;
            AddParameter(new Parameter("Width", ParameterType.Constant, 0));
        }
        /// <summary>
        /// Width
        /// </summary>
        public double Width 
        {
            get { return this["Width"].GetValue(); }
            set { this["Width"].SetValue(value); } 
        }

        /// <summary>
        /// IsPointInside
        /// </summary>
        /// <param name="elementCenter - отображенные в локальную ск"></param>
        /// <param name="rad"></param>
        /// <param name="halfBead"></param>
        /// <returns></returns>
        public bool IsPointInside(Point3D elementCenter, out double rad, out double halfBead)
        {
            halfBead = Width / 2;
            var frame = new Frame();

            rad = Vector.GetVectorLength(elementCenter.Sub(frame.Centre));
            return rad <= halfBead;
        }

        /// <inheritdoc/>

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();

            foreach (var element in elements)
                if (IsPointInside(element.CalcCentr(), out _, out _))
                    insideElements.Add(element);

            return insideElements;
        }
        /// <inheritdoc/>

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= Width / 2;
        }
        /// <inheritdoc/>

        public override double CalcValue(Point3D elementCenter)
        {
            double rad, halfBead;

            if (IsPointInside(elementCenter, out rad, out halfBead))
            {
                // мощность берется за единицу
                var power = 1;
                var vol = halfBead * halfBead * halfBead;
                var powerDensity = power / vol;

                return powerDensity * Math.Exp(-1 * (rad * rad / (halfBead * halfBead)));
            }
            return 0;
        }

        public override double CalcValue()
        {
            double rad, halfBead;

            var x = (float)this["X"].GetValue();
            var y = (float)this["Y"].GetValue();
            var z = (float)this["Z"].GetValue();

            var elementCenter = new Point3D(x, y, z);

            if (IsPointInside(elementCenter, out rad, out halfBead))
            {
                // мощность берется за единицу
                var power = 1;
                var vol = halfBead * halfBead * halfBead;
                var powerDensity = power / vol;

                return powerDensity * Math.Exp(-1 * (rad * rad / (halfBead * halfBead)));
            }
            return 0;
        }
    }
}
