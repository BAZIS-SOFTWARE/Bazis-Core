using Geometry;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Utilities
{
    public class SurfacesFinder
    {
        private const double epsilon = 1e-6;

        /// <param name="nodes">Узлы, задающие выпуклую 3D модель</param>
        /// <returns>Перечисление поверхностей, составляющих грани 3D модели</returns>
        public static IEnumerable<T[]> FindSurfaceNodes<T>(T[] nodes) where T : IPoint
        {
            var usedSurfaces = new List<bool[]>();
            foreach (var indexes in GetSubsets(nodes))
            {
                if (usedSurfaces.Any(arr => arr[indexes[0]] && arr[indexes[1]] && arr[indexes[2]])) // проверяем использована ли плоскость по индексам узлов
                    continue;
                var plane = new Plane(nodes[indexes[0]].Position, nodes[indexes[1]].Position, nodes[indexes[2]].Position);
                var surfaceNodeIndeces = new List<int> { indexes[0], indexes[1], indexes[2] };
                var signs = new List<int>();
                for (var i = 0; i < nodes.Length; i++)
                {
                    if (i == indexes[0] || i == indexes[1] || i == indexes[2]) // отсеиваем узлы образующие текущую плоскость поиска
                        continue;
                    var distance = plane.PointPosition(nodes[i].Position);
                    if (Math.Abs(distance) < epsilon) // если узел лежит в плоскости
                        surfaceNodeIndeces.Add(i);
                    else
                    {
                        var currentSign = Math.Sign(plane.PointPosition(nodes[i].Position));//если узел не в плоскости
                        signs.Add(currentSign);
                    }
                }
                if (signs.All(x => x > 0) || signs.All(x => x < 0)) //проверка граничная ли это плоскость
                {
                    var used = new bool[nodes.Length];
                    surfaceNodeIndeces.ForEach(i => used[i] = true);
                    usedSurfaces.Add(used);
                    yield return surfaceNodeIndeces.Select(i => nodes[i]).ToArray();
                }
            }
        }

        /// <summary>
        /// Перечисляет все трёхэлементные подмножества заданного множества.
        /// Необходимо для перебора из заданного множества узлов троек для построения плоскости
        /// </summary>
        private static IEnumerable<int[]> GetSubsets<T>(T[] set)
        {
            for (var i = 0; i < set.Length - 2; i++)
            {
                for (var j = i + 1; j < set.Length - 1; j++)
                {
                    for (var k = j + 1; k < set.Length; k++)
                    {
                        yield return new[] { i, j, k };
                    }
                }
            }
        }
    }
}