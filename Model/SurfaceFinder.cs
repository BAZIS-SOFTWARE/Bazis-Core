using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.MeshObjects;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsSorters
{
    public class SurfaceFinder
    {
        /// <param name="nodes">Узлы, задающие выпуклую 3D модель</param>
        /// <returns>Перечисление поверхностей, составляющих грани 3D модели</returns>
        public static IEnumerable<ISurface> FindSurfaces(Node[] nodes)
        {
            var surfaces = new List<HashSet<Node>>();
            var surfaceId = 0;
            foreach (var subset in GetSubsets(nodes))
            {
                if (IsSubsetUsed(surfaces, subset.ToHashSet()))
                    continue;
                var otherNodes = nodes.ToHashSet().Except(subset);
                var points = subset.Select(x => x.Position).ToArray();
                var plane = new Plane(points[0], points[1], points[2]);
                //var equationCoefficients = GetPlaneEquationCoefficients(points[0], points[1], points[2]);
                var accept = true;
                var initialSign = plane.PointPosition(otherNodes.First().Position) >= 0;
                //var initialSign = GetDistance(equationCoefficients, otherNodes.First().Position) >= 0;
                var potentialSurface = new List<Node>();
                foreach (var node in otherNodes)
                {
                    var distance = plane.PointPosition(node.Position);
                    var currentSign = (distance >= 0);
                    if (Math.Abs(distance) < 1e-6)
                        potentialSurface.Add(node);
                    else if (currentSign != initialSign)
                    {
                        accept = false;
                        break;
                    }
                }
                if (accept)
                {
                    potentialSurface.AddRange(subset);
                    surfaces.Add(potentialSurface.ToHashSet());
                    yield return new Surface(surfaceId, potentialSurface.ToArray());
                    surfaceId++;
                }
            }
        }

        /// <summary>
        /// Перечисляет все трёхэлементные подмножества заданного множества.
        /// Необходимо для перебора из заданного множества узлов троек для построения плоскости
        /// </summary>
        private static IEnumerable<T[]> GetSubsets<T>(T[] set)
        {
            for (int i = 0; i < set.Length - 2; i++)
            {
                for (int j = i + 1; j < set.Length - 1; j++)
                {
                    for (int k = j + 1; k < set.Length; k++)
                    {
                        yield return new T[] { set[i], set[j], set[k] };
                    }
                }
            }
        }

        /// <summary>
        ///  Вычисляет определитель заданной матрицы
        /// </summary>    
        private static float GetDet(float[,] matrix)
        {
            var A = Matrix<float>.Build.DenseOfArray(matrix);
            return A.Determinant();
        }

        ///// <summary>
        ///// Вычисляет коэффициенты A, B, C, D уравнения плоскости Ax + By + Cz + D = 0,
        ///// заданной двумя точками
        ///// </summary>
        //private static float[] GetPlaneEquationCoefficients(Point3D a, Point3D b, Point3D c)
        //{
        //    var A = GetDet(new float[,] { { b._y - a._y, b._z - a._z }, 
        //                                  { c._y - a._y, c._z - a._z } });
        //    var B = GetDet(new float[,] { { b._x - a._x, b._z - a._z },
        //                                  { c._x - a._x, c._z - a._z } });
        //    var C = GetDet(new float[,] { { b._x - a._x, b._y - a._y },
        //                                    { c._x - a._x, c._y - a._y } });
        //    var D = -a._x * A + a._y * B - a._z * C;
        //    return new float[4] { A, -B, C, D };
        //}

        ///// <summary>
        ///// Вычисляет расстояние от заданной точки, до плоскости, задаваемой коэффициентами
        ///// уравнения Ax + By + Cz + D = 0
        ///// </summary>
        //private static double GetDistance(float[] equationCoefficients, Point3D p)
        //{
        //    var x = p._x;
        //    var y = p._y;
        //    var z = p._z;
        //    var A = equationCoefficients[0];
        //    var B = equationCoefficients[1];
        //    var C = equationCoefficients[2];
        //    var D = equationCoefficients[3];
        //    return (A * x + B * y + C * z + D) / Math.Sqrt(A * A + B * B + C * C);
        //}

        /// <summary>
        /// Определяет, находится ли подмножество в списке используемых подмножеств
        /// </summary>
        private static bool IsSubsetUsed(List<HashSet<Node>> usedSubsets, HashSet<Node> subset)
        {
            foreach (var item in usedSubsets)
            {
                if (!subset.Except(item).Any())
                    return true;
            }
            return false;
        }
    }
}