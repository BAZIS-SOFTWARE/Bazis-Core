namespace Geometry.Interfaces
{
    /// <summary>
    /// Plane interface
    /// </summary>
    public interface IPlane
    {
        /// <summary>
        /// Normal
        /// </summary>
        Point3D Normal { get; }
        /// <summary>
        /// Shifting
        /// </summary>
        double Shifting { get; set; }

        /// <summary>
        /// PointPosition
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        double PointPosition(Point3D point);
        /// <summary>
        /// PlanePosition
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        double PlanePosition(Plane plane);
    }
}
