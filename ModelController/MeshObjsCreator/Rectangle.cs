using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    internal class Rectangle
    {
        public float Left { get; }

        public float Right { get; }

        public float Bottom { get; }

        public float Top { get; }

        public float Height => Top - Bottom;

        public float Width => Right - Left;

        public Rectangle(float left, float right, float bottom, float top)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }
    }
}
