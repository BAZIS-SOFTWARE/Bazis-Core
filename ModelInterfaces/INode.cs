using Geometry;

namespace ModelInterfaces
{
    public interface INode : IModelObject
    {
        Point3D Position { get; }
        Point3D Normal { get; set; }
    }
}