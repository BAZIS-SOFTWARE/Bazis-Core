using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// ISurface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISurface<T> : INormal,ISquare, IVertexObject<T>
        where T : IPoint
    {
        /// <summary>
        /// Поменять порядок обхода узлов
        /// </summary>
        void ReverseVertexes();
        /// <summary>
        /// GetDistance
        /// </summary>
        /// <param name="point3D"></param>
        /// <returns></returns>
        float GetDistance(Point3D point3D);
        /// <summary>
        /// GetLinesFromSurface
        /// </summary>
        /// <returns></returns>
        Segment3D[] GetLinesFromSurface();
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        T this[int ind] { get; }
    }
}
