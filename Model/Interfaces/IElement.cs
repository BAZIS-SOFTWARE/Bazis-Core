using Geometry;

namespace Model.Interfaces
{
    public interface IElement : INodeObject
    {
        float[] GetFormFunctions(Point3D point3D);

        float[] GetDerivativesFormFunctions(Point3D point3D);
    }
}
