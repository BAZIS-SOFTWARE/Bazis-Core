using Geometry;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
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
            foreach (var subset in GetSubsets(nodes))
            {
                if (usedSurfaces.Any(arr => arr[subset[0]] && arr[subset[1]] && arr[subset[2]]))
                    continue;
                var plane = new Plane(nodes[subset[0]].Position, nodes[subset[1]].Position, nodes[subset[2]].Position);
                var surfaceNodeIndeces = new List<int> { subset[0], subset[1], subset[2] };
                var signs = new List<int>();
                for (var i = 0; i < nodes.Length; i++)
                {
                    if (i == subset[0] || i == subset[1] || i == subset[2])
                        continue;
                    var distance = plane.PointPosition(nodes[i].Position);
                    if (Math.Abs(distance) < epsilon)
                        surfaceNodeIndeces.Add(i);
                    else
                    {
                        var currentSign = Math.Sign(plane.PointPosition(nodes[i].Position));
                        signs.Add(currentSign);
                    }
                }
                if (signs.All(x => x > 0) || signs.All(x => x < 0))
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