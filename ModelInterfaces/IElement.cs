using Geometry;

namespace ModelInterfaces
{
    public interface IElement : INodeObject
    {
        float[] GetFormFunctions(Point3D point3D);

        float[] GetDerivativesFormFunctions(Point3D point3D);

        float DetJ(Point3D point3D);
    }
}
