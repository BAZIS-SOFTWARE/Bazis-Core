using Geometry;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// ITrajectory
    /// </summary>
    public interface ITrajectory
    {
        Point3D this[int ind] { get; }
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// SetPoints
        /// </summary>
        /// <param name="points"></param>
        void SetPoints(IEnumerable<Point3D> points);
        /// <summary>
        /// GetPoints
        /// </summary>
        /// <returns></returns>
        IEnumerable<Point3D> GetPoints();
        /// <summary>
        /// NumberOfPoints
        /// </summary>
        int NumberOfPoints { get; }
    }
}