namespace SurfaceMesherInterfaces
{
    public interface IPoint2D
    {
        double X { get; }

        double Y { get; }

        IPoint2D Sub(IPoint2D b);

        IPoint2D Add(IPoint2D b);

        IPoint2D Mult(double factor);

        double GetLength();
    }
}