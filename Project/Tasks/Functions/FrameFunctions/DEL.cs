using Geometry;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class DEL : FrameFunction
    {
        /// <summary>
        /// DEL
        /// </summary>
        public DEL()
        {
            Name = FuncName.DEL.ToString();
            FunctionType = FuncType.CPF;
            AddParameter(new Parameter("Width", ParameterType.Constant, 0));
            AddParameter(new Parameter("Depth", ParameterType.Constant, 0));
            AddParameter(new Parameter("FrontPart", ParameterType.Constant, 0));
            AddParameter(new Parameter("BackPart", ParameterType.Constant, 0));
        }

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
            var vectorLength = Vector.GetVectorLength(elementCenter);
            if (vectorLength <= MinSemiaxis) return true;
            else if (vectorLength > MaxSemiaxis) return false;

            float semiEllipsoid;
            if (elementCenter._y < 0)
                semiEllipsoid = BackPart;
            else semiEllipsoid = FrontPart;

            var result = Math.Pow(elementCenter._x / (Width / 2), 2) +
                Math.Pow(elementCenter._y / semiEllipsoid, 2) +
                Math.Pow(elementCenter._z / Depth, 2) - 1;

            return result <= 0;

        }

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= (FrontPart + BackPart) / 2;
        }

        public override double CalcValue(Point3D elementCenter)
        {
            throw new NotImplementedException();
        }

        public override double CalcValue()
        {
            throw new NotImplementedException();
        }
    }
}
