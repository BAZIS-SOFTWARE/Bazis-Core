namespace SurfaceMesher 
{
    /// <summary>
    /// Четырёхугольный элемент сетки, полученный объединением двух треугольников
    /// </summary>
    public class Quad2D
    {
        public int Index { get; }
        public Node2D A { get; }
        public Node2D B { get; }
        public Node2D C { get; }
        public Node2D D { get; }

        public Quad2D(int index, Node2D a, Node2D b, Node2D c, Node2D d)
        {
            Index = index;
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}