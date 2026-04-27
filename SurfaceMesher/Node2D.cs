namespace SurfaceMesher
{
    /// <summary>
    /// Узел на плоскости
    /// </summary>
    public class Node2D
    {
        public int Index { get; }

        public Point2D Position { get; }     

        public double X => Position.X;

        public double Y => Position.Y;

        public Node2D(int index, Point2D position)
        {
            Index = index;
            Position = position;
        }

        public Node2D(int index, double x, double y) : this(index, new Point2D(x, y)) { }

        public override bool Equals(object obj)
        {
            if(obj is Node2D other)
                return Index == other.Index;
            return false;
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public override string ToString()
        {
            return $"{Index}: {Position}";
        }
    }
}
