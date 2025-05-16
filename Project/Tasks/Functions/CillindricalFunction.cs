using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    public abstract class CillindricalFunction : FrameFunction
    {
        /// <inheritdoc/>
        public float BottomDiam { get;  set; }
        /// <inheritdoc/>
        public float UpperDiam { get;  set; }
        /// <inheritdoc/>
        public float Length { get;  set; }

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

            var hsPlane = new Plane(Frame.Dir_X, Frame.Centre, Frame.Dir_Y);
            var res = hsPlane.PointPosition(elementCenter);

            if (res < 0 | res > Length)
                return false;

            var hsAxis = new Segment3D(Frame.Centre, Frame.Dir_Z);
            r = elementCenter.GetDistanceToLine(hsAxis);

            var elDist = elementCenter.Sub(Frame.Centre);
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

        public override bool IsOverlappingSelf( float velocity)
        {
            return velocity <= BottomDiam / 2 && velocity <= UpperDiam / 2;
        }
    }
}
