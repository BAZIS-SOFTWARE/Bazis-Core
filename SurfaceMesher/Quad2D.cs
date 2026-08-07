namespace SurfaceMesher 
{
    /// <summary>
    /// Четырёхугольный элемент сетки, полученный объединением двух треугольников
    /// </summary>
    public class Quad2D : Element
    {
        public int Index { get; }
        public Node2D A { get { return nodes[0]; } }

        public Node2D B { get { return nodes[1]; } }

        public Node2D C { get { return nodes[2]; } }

        public Node2D D { get { return nodes[3]; } }

        public Quad2D(int index, Node2D a, Node2D b, Node2D c, Node2D d)
        {
            Index = index;
            nodes = new Node2D[4];

            nodes[0] = a; nodes[1] = b; nodes[2] = c; nodes[3] = d;

            // Возможно нужна сортировка!!!!
        }
    }
}