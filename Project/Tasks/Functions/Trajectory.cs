using Geometry;
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    public class Trajectory : ITrajectory
    {
        /// <summary>
        /// Trajectory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="points"></param>
        public Trajectory(string name, List<Point3D> points) : this(name)
        {
            Name = name;
            this.points = new Curve3D(points);           
        }

        public Point3D this [int ind]
        {
            get { return points[ind]; }
        }

        public Trajectory(string name)
        {
            Name = name;
            points = new Curve3D();
        }
/// <inheritdoc/>

        public string Name { get; private set; }

        private Curve3D points;
/// <inheritdoc/>

        public IEnumerable<Point3D> GetPoints()
        {
            foreach (var point in points)
            {
                yield return point;
            }
        }
/// <inheritdoc/>

        public void SetPoints(IEnumerable<Point3D> points)
        {
            this.points.AddRange(points);
        }
/// <inheritdoc/>

        public int NumberOfPoints { get { return points.Count; } }
/// <inheritdoc/>

        public override string ToString()
        {
            return Name;
        }
    }
}
