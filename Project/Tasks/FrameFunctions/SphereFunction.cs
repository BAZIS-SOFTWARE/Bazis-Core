using System;
using System.Collections.Generic;
using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks.FrameFunctions
{
    public class SphereFunction : FrameFunction
    {
        public SphereFunction()
        {
            Name = FrameFunctionName.SPH.ToString();
        }

        public override string ToString()
        {
            return $"{base.ToString()};{Width}";
        }
        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementCenter"></param>
        /// <param name="rad"></param>
        /// <param name="halfBead"></param>
        /// <returns></returns>
        public bool IsPointInside(Point3D elementCenter, out double rad, out double halfBead)
        {
            halfBead = Width / 2;
            var frame = LocalFrame.Frame;
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
    }
}
