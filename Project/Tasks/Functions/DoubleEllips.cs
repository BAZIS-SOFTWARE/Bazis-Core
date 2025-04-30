using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;


namespace Project.Tasks.Functions
{
    public class DoubleEllips : FrameFunction
    {
        public string Name { get; set; }

        /// <summary>
        /// Ширина сварочной ванны, мм
        /// </summary>
        public float Width { get; set; } // по оси X

        /// <summary>
        /// Глубина сварочной ванны, мм
        /// </summary>
        public float Depth { get; set; } // по оси Z

        /// <summary>
        /// Длина передней части сварочной ванны, мм
        /// </summary>
        public float FrontPart { get; set; } // по оси Y

        /// <summary>
        /// Длина хвостовой части сварочной ванны, мм
        /// </summary>
        public float BackPart { get; set; } // по оси Y


        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();

            foreach (var element in elements)
                if (IsPointInside(element.CalcCentr()))
                    insideElements.Add(element);

            return insideElements;
        }

        public bool IsPointInside(Point3D elementCenter)
        {
            var desiredPoint = elementCenter.Sub(Frame.Centre);

            float semiEllipsoid;
            if (elementCenter._y < Frame.Centre._y)
                semiEllipsoid = BackPart;
            else semiEllipsoid = FrontPart;

            var result = Math.Pow(desiredPoint._x / (Width / 2), 2) + Math.Pow(desiredPoint._y / semiEllipsoid, 2) +
                Math.Pow(desiredPoint._z / Depth, 2);

            return result <= 1;
        }

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= (FrontPart + BackPart) / 2;
        }

        public override float CalcPower(Point3D elementCenter)
        {
            throw new NotImplementedException();
        }
    }
}
