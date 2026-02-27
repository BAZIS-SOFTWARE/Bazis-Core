using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Project.Tasks.FrameFunctions
{
    public class CillindricalFunction : FrameFunction
    {
        /// <inheritdoc/>
        public double BottomDiam { get; set; }
        /// <inheritdoc/>
        public double UpperDiam { get; set; }
        /// <inheritdoc/>
        public double Length { get; set; }

        public CillindricalFunction()
        {
            Name = FrameFunctionName.CIL.ToString();
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{base.ToString()};{Length};{UpperDiam};{BottomDiam}";
        }


        /// <summary>
        /// IsPointInside
        /// </summary>
        /// <param name="elementCenter"></param>
        /// <param name="r"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public bool IsPointInside(Point3D elementCenter, out double r, out double r0, out double r1, out double r2)
        {
            r = 0f;
            r0 = 0f;
            r1 = 0f;
            r2 = 0f;
            var frame = LocalFrame.Frame;

            // получаем плоскость верхней повер-ти фигуры
            var hsPlane = new Plane(frame.Centre, frame.Dir_Z);

            var res = hsPlane.PointPosition(elementCenter);

            if (res > 0 | res < -Length)
                return false;

            var elPlane = new Plane(elementCenter, frame.Dir_Z);

            var elP = frame.Centre.GetPointProectionOnPlane(elPlane);

            var elAxis = new Segment3D(frame.Centre, elP);

            r1 = UpperDiam / 2;
            r2 = BottomDiam / 2;

            var l = elAxis.GetLength();
            r0 = r2 + (Length - l) * (r1 - r2) / Length;

            var elSeg = new Segment3D(elP, elementCenter);
            r = elSeg.GetLength();

            if (r - r0 > 1e-4)
                return false;

            return true;
        }
        /// <inheritdoc/>

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();

            foreach (var element in elements)
                if (IsPointInside(element.CalcCentr(), out _, out _, out _, out _))
                    insideElements.Add(element);

            return insideElements;
        }
        /// <inheritdoc/>

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= BottomDiam / 2 && velocity <= UpperDiam / 2;
        }
/// <inheritdoc/>

        public override double CalcValue(Point3D elementCenter)
        {
            double r, r0, r1, r2;

            if (IsPointInside(elementCenter, out r, out r0, out r1, out r2))
            {
                var power = 1;
                var vol = 1.0f / 3 * 3.14f * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var powerDensity = power / vol;

                var relPowerDensity = powerDensity * Math.Exp(-1 * (r * r / (r0 * r0)));
                return relPowerDensity;
            }
            return 0;
        }
    }
}
