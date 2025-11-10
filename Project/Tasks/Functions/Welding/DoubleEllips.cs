using Geometry;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions.Welding
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

        //public Matrix<float> InverseTransitionMatrix { get; set; }

        private float MaxSemiaxis { get; set; }
        private float MinSemiaxis { get; set; }

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();
            //DetermineInverseMatrix();
            MaxAndMinSemiaxis();

            foreach (var element in elements)
                if (IsPointInside(element.CalcCentr()))
                    insideElements.Add(element);

            return insideElements;
        }

        public void MaxAndMinSemiaxis()
        {
            var Max1 = Math.Max(Depth, Width / 2);
            var Min1 = Math.Min(Depth, Width / 2);
            MaxSemiaxis = Math.Max(Max1, BackPart);
            MinSemiaxis = Math.Min(Min1, FrontPart);
        }


        public bool IsPointInside(Point3D elementCenter)
        {
            var frame = LocalFrame.Frame;
            var elementVector = elementCenter.Sub(frame.Centre);
            var vectorLength = Geometry.Vector.GetVectorLenght(elementVector);
            if (vectorLength <= MinSemiaxis) return true;
            else if (vectorLength > MaxSemiaxis) return false;

            var pointInEllipsCoordinate = frame.GetCoordsInFrame(elementCenter);

            float semiEllipsoid;
            if (pointInEllipsCoordinate._y < 0)
                semiEllipsoid = BackPart;
            else semiEllipsoid = FrontPart;

            var result = Math.Pow(pointInEllipsCoordinate._x / (Width / 2), 2) +
                Math.Pow(pointInEllipsCoordinate._y / semiEllipsoid, 2) +
                Math.Pow(pointInEllipsCoordinate._z / Depth, 2) - 1;

            return result <= 0;

        }

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= (FrontPart + BackPart) / 2;
        }

        public override float CalcValue(Point3D elementCenter)
        {
            throw new NotImplementedException();
        }
    }
}
