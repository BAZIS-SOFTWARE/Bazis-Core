using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace ModelController.MeshObjsCreator.Delone2D
{
    public class DeloneNode2D
    {
        public int Number { get; }

        public Point2D Position { get; }

        public DeloneNode2D(int number, Point2D position)
        {
            Number = number;
            Position = position;
        }

        public override string ToString()
        {
            return $"{Number} {Position}";
        }
    }
}
