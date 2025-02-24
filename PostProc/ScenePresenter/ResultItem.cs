using Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostProc.ScenePresenter
{
    public class ResultItem
    {
        public Point3D Coord { get; set; }
        public float Value { get; set; }

        public ResultItem(Point3D coord, float value)
        {
            Coord = coord;
            Value = value;
        }

        public ResultItem()
        {

        }

        public override string ToString()
        {
            return $"{Coord} : {Value}";
        }
    }
}
