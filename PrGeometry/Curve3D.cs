using System;
using System.Collections;
using System.Collections.Generic;

namespace Geometry
{
    /// <summary>
    /// Curve3D
    /// </summary>
    public class Curve3D : IList<Point3D>
    {
        List<Point3D> points = new List<Point3D>();
        /// <summary>
        /// Curve3D
        /// </summary>
        /// <param name="points"></param>
        public Curve3D(List<Point3D> points) : base ()
        {
            this.points = points;
        }

        public Curve3D()
        {
            points = new List<Point3D>();
        }
        /// <summary>
        /// Indexator
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point3D this[int index] 
        { 
            get { return points[index]; }
            set { points[index] = value; }
        }
/// <inheritdoc/>

        public int Count
        {
            get { return points.Count; }
        }
/// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }
/// <inheritdoc/>

        public void Add(Point3D point)
        {
            points.Add(point);
        }
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="points"></param>
        public void AddRange(IEnumerable<Point3D> points)
        {
            this.points.AddRange(points);
        }
/// <inheritdoc/>

        public void Clear()
        {
            points.Clear();
        }
/// <inheritdoc/>

        public bool Contains(Point3D item)
        {
            return points.Contains(item);
        }
/// <inheritdoc/>

        public void CopyTo(Point3D[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerator<Point3D> GetEnumerator()
        {
            foreach (var point in points)
            {
                yield return point;
            }
        }
/// <inheritdoc/>

        public int IndexOf(Point3D point)
        {
            return points.IndexOf(point);
        }
/// <inheritdoc/>

        public void Insert(int index, Point3D item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public bool Remove(Point3D point)
        {
            return points.Remove(point);
        }
/// <inheritdoc/>

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
