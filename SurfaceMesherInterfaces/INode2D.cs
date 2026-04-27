namespace SurfaceMesherInterfaces
{
    public interface INode2D
    {
        int Index { get; }

        IPoint2D Position { get; }

        double X { get; }

        double Y { get; }
    }
}