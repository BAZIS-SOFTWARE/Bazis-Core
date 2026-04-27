namespace SurfaceMesherInterfaces
{
    public interface IEdge
    {
        INode2D A { get; }

        INode2D B { get; }

        bool IsIntersectWith(IEdge other);

        IPoint2D GetMiddlePoint();
    }
}