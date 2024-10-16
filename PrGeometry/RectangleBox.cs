using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class RectangleBox
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }

        public RectangleBox(int left, int right, int bottom, int top)
        {
            Right = right;
            Left = left;

            if (left > right)
            {
                Right = left;
                Left = right;
            }

            Bottom = bottom;
            Top = top;

            if (bottom > top)
            {
                Bottom = top;
                Top = bottom;
            }


        }

        public bool IsIntersectWithOther(RectangleBox other)
        {
            return Left <= other.Right && Right >= other.Left &&
                   Top >= other.Bottom && Bottom <= other.Top;
        }

        public bool IsInnerOther(RectangleBox other)
        {
            if (other.Left <= Left && Right <= other.Right
                                   && other.Top >= Top
                                   && Bottom >= other.Bottom)
                return true;
            return false;
        }

        public Point2D[] GetPoints()
        {
            return new[]
            {
                new Point2D(Left, Top), new Point2D(Left, Bottom),
                new Point2D(Right, Bottom), new Point2D(Right, Top)
            };
        }

        public bool IsPointsInside(Point2D[] points)
        {
            var counter = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i]._x > Left - 5 &&
                points[i]._x < Right &&
                points[i]._y > Bottom &&
                points[i]._y < Top + 5)
                {
                    counter++;
                }
            }

            if (counter == points.Length)
                return true;
            else
                return false;
        }

        public bool IsPointInside(Point2D point)
        {

            if (point._x > Left - 5 &&
            point._x < Right &&
            point._y > Bottom &&
            point._y < Top + 5)
            {
                return true;
            }
            else
                return false;
        }
    }
}
