using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace ModelController.MeshObjsCreator
{
    internal class Node2D
    {
        public int Number { get; }

        public Point2D Position { get; }

        public Node2D(int number, Point2D position)
        {
            Number = number;
            Position = position;
        }
    }
}
