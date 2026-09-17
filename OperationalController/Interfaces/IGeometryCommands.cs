using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции изменения геометрической модели.
    /// </summary>
    public interface IGeometryCommands
    {
        void InitializeGeometry(string gmshDllPath);
        void UnloadGeometry();
        void DeleteGeometryObject(int dim, int number, bool recursive = false);
        int CreatePointByVector(int basePointTag, int directionPointTag, double offset);
        int CreatePointProjectionOntoGeometry(int basePointTag, int dim, int geometryTag);
        int CreatePoint(double x, double y, double z, double meshSize = 0);
        int CreateLine(int startPointNumber, int endPointNumber);
        int CreateSurface(int[] lineNumbers);
        int[] CreateChamfer(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected);
        void SetEmbeddedGeometryEntities(int hostDimension, int hostNumber, int embeddedDimension, IEnumerable<int> embeddedNumbers);
    }
}
