using Geometry;

namespace ModelInterfaces.MeshObjects
{
    /// <summary>
    /// IElement2D
    /// </summary>
    public interface IElement2D : ISurfaceElement, ISquare
    {
        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
        void CreateIntegrationPoint(int w, double ksi, double eta);

    }
}
