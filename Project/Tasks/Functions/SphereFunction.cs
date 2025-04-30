using System.Collections.Generic;
using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks.Functions
{
    public abstract class SphereFunction : FrameFunction
    {
        /// <inheritdoc/>
        public float Width { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementCenter"></param>
        /// <param name="rad"></param>
        /// <param name="halfBead"></param>
        /// <returns></returns>
        public bool IsPointInside(Point3D elementCenter, out float rad, out float halfBead)
        {
            halfBead = Width / 2;
            rad = Vector.GetVectorLenght(elementCenter.Sub(Frame.Centre));
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
    }
}
