using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    /// <summary>
    /// ISurface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISurface<T> : INormal,ISquare
        where T : IPoint
    {
        //Position3D Position
        /// <summary>
        /// Number
        /// </summary>
        int Number { get; }
        /// <summary>
        /// NumberOfPoints
        /// </summary>
        int NumberOfPoints { get; }
        /// <summary>
        /// View state flag
        /// </summary>
        bool ViewState { get; set; }
        /// <summary>
        /// GetVertexes
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetVertexes();
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
