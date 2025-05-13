using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra;
using Model.IO;
using CsvHelper;
using Model.IO.STL;

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

        public Matrix<float> InverseTransitionMatrix { get; set; }

        private float MaxSemiaxis { get; set; }
        private float MinSemiaxis { get; set; }

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();
            DetermineInverseMatrix();
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

        public void DetermineInverseMatrix()
        {            
            var frameArr = Matrix<float>.Build.DenseOfArray(new float[3, 3]
                {{Frame.Dir_X._x, Frame.Dir_X._y, Frame.Dir_X._z  },
                 {Frame.Dir_Y._x, Frame.Dir_Y._y, Frame.Dir_Y._z},
                 {Frame.Dir_Z._x, Frame.Dir_Z._y, Frame.Dir_Z._z}});

            var bazisMatrix = Matrix<float>.Build.DenseOfArray(new float[3, 3] {
                                                      {1, 0, 0},
                                                      {0, 1, 0},
                                                      {0, 0, 1}});

            var transposedFrameArr = frameArr.Transpose();
            var transposedBazisMatrix = bazisMatrix.Transpose();
            var inverseBazisMatrix = transposedBazisMatrix.Inverse();
            var transitionMatrix = inverseBazisMatrix * transposedFrameArr;

            if (transitionMatrix.Determinant() == 0)          
                throw new StlFileLoadExeption("Матрица вырождена, обратной матрицы не существует", new Exception());
            else 
                InverseTransitionMatrix = transitionMatrix.Inverse();           
        }


        public bool IsPointInside(Point3D elementCenter)
        {
            var displayPoint = elementCenter.Sub(Frame.Centre);
            var vectorLength = Geometry.Vector.GetVectorLenght(displayPoint);
            if (vectorLength <= MinSemiaxis) return true;
            else if (vectorLength > MaxSemiaxis) return false;

            var desiredPointMatrix = Matrix<float>.Build.DenseOfArray(new float[1, 3]{
                                                                { displayPoint._x, displayPoint._y, displayPoint._z } });

            var pointInEllipsCoordinate = desiredPointMatrix * InverseTransitionMatrix;            

            float semiEllipsoid;
            if (elementCenter._y < Frame.Centre._y)
                semiEllipsoid = BackPart;
            else semiEllipsoid = FrontPart;

            var result = Math.Pow(pointInEllipsCoordinate[0, 0] / (Width / 2), 2) +
                Math.Pow(pointInEllipsCoordinate[0, 1] / semiEllipsoid, 2) +
                Math.Pow(pointInEllipsCoordinate[0, 2] / Depth, 2);

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
