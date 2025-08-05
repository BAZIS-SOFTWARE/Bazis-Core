using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions.Welding
{
    public class CillindricalFunction : FrameFunction
    {
        /// <inheritdoc/>
        public float BottomDiam { get; set; }
        /// <inheritdoc/>
        public float UpperDiam { get; set; }
        /// <inheritdoc/>
        public float Length { get; set; }

        /// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name};{Length};{UpperDiam};{BottomDiam};{LocalFrame}";
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
        public bool IsPointInside(Point3D elementCenter, out float r, out float r0, out float r1, out float r2)
        {
            r = 0f;
            r0 = 0f;
            r1 = 0f;
            r2 = 0f;
            var frame = LocalFrame.Frame;

            var p_x = frame.Centre.Sum(frame.Dir_X);
            var p_y = frame.Centre.Sum(frame.Dir_Y);
            var hsPlane = new Plane(p_x, frame.Centre, p_y);

            var res = hsPlane.PointPosition(elementCenter);

            if (res < 0 | res > Length)
                return false;

            var hsAxis = new Segment3D(frame.Centre, frame.Dir_Z);
            r = elementCenter.GetDistanceToLine(hsAxis);

            var elDist = elementCenter.Sub(frame.Centre);
            var rad = Vector.GetVectorLenght(elDist);

            var zc = 0.0f;
            if (rad - r > 0)
                zc = (float)Math.Sqrt(rad * rad - r * r);

            r1 = UpperDiam / 2;
            r2 = BottomDiam / 2;
            r0 = r1 + zc * (r2 - r1) / Length; // pin rad

            return r - r0 <= 1e-4;
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

        public override float CalcValue(Point3D elementCenter)
        {
            float r, r0, r1, r2;

            if (IsPointInside(elementCenter, out r, out r0, out r1, out r2))
            {
                var power = 1;
                var vol = 1.0f / 3 * 3.14f * Length * (r1 * r1 + r1 * r2 + r2 * r2); // объем усеченного конуса
                var powerDensity = power / vol;

                var relPowerDensity = powerDensity * (float)Math.Exp(-1 * (r * r / (r0 * r0)));
                return relPowerDensity;
            }
            return 0;
        }
    }
}
