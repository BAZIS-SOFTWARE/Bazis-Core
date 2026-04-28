using System;
using System.Collections.Generic;

namespace SurfaceMesher
{
    /// <summary>
    /// Треугольник на плоскости
    /// </summary>
    public class Triangle2D
    {
        private const double epsilon = 1e-4;

        public Node2D A { get; }

        public Node2D B { get; }

        public Node2D C { get; }

        public int Index { get; }

        public Triangle2D(int index, Node2D a, Node2D b, Node2D c)
        {
            A = a;
            B = b;
            C = c;
            Index = index;
            CheckCounterClockwiseOrder();
        }

        /// <summary>
        /// Получить площадь треугольника
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            var ac = C.Position.Sub(A.Position);
            var ab = B.Position.Sub(A.Position);
            var crossProduct = Vector.CrossProduct(ac, ab); //edit
            return 0.5 * Math.Abs(crossProduct);
        }

        public double GetQuality()
        {
            var S = GetArea();
            var a = Vector.GetSquaredVectorLength(B.Position.Sub(A.Position)); //edit
            var b = Vector.GetSquaredVectorLength(C.Position.Sub(B.Position));
            var c = Vector.GetSquaredVectorLength(A.Position.Sub(C.Position));
            var L = a + b + c;
            return 4 * Math.Sqrt(3) * S / L;
        }

        public IEnumerable<Node2D> GetNodes()
        {
            return [A, B, C];
        }

        public Point2D GetCentroid()
        {
            var x = (A.Position.X + B.Position.X + C.Position.X) / 3f;
            var y = (A.Position.X + B.Position.X + C.Position.X) / 3f;
            return new Point2D(x, y);
        }

        public override string ToString()
        {
            return $"Index: {Index}; Nodes: {A.Index}; {B.Index}; {C.Index}";
        }

        /// <summary>
        /// Проверка на обход вершин против часовой стрелки
        /// </summary>
        private void CheckCounterClockwiseOrder()
        {
            var vectorOne = B.Position.Sub(A.Position);
            var vectorTwo = C.Position.Sub(A.Position);
            var crossProduct = Vector.CrossProduct(vectorOne, vectorTwo); //edit
            if(Math.Abs(crossProduct) <= epsilon)
                throw new ArgumentException($"Вырожденный или слишком маленький треугольник {Index}");
            if (crossProduct < 0)
                throw new ArgumentException($"Узлы треугольника {Index} должны быть упорядочены против часовой стрелки");
        }
    }
}