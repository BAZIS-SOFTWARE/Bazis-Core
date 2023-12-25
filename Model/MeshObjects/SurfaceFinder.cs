using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class SurfaceFinder
    {
        /// <param name="nodes">Узлы, задающие выпуклую 3D модель</param>
        /// <returns>Перечисление поверхностей, составляющих грани 3D модели</returns>
        public static IEnumerable<T[]> FindSurfaceNodes<T>(T[] nodes) where T : IPoint
        {
            var surfaces = new List<HashSet<T>>();
            foreach (var subset in GetSubsets(nodes))
            {
                if (IsSubsetUsed(surfaces, subset.ToHashSet()))
                    continue;
                var otherNodes = nodes.ToHashSet().Except(subset).ToList();
                var points = subset.Select(x => x.Position).ToArray();
                var plane = new Plane(points[0], points[1], points[2]);
                var accept = true;
                var initialSign = plane.PointPosition(otherNodes.First().Position) >= 0;
                var surface = new List<T>();
                foreach (var node in otherNodes)
                {
                    var distance = plane.PointPosition(node.Position);
                    var currentSign = distance >= 0;
                    if (Math.Abs(distance) < 1e-6)
                        surface.Add(node);
                    else if (currentSign != initialSign)
                    {
                        accept = false;
                        break;
                    }
                }
                if (accept)
                {
                    surface.AddRange(subset);
                    surfaces.Add(surface.ToHashSet());
                    yield return surface.ToArray();
                }
            }
        }

        /// <summary>
        /// Перечисляет все трёхэлементные подмножества заданного множества.
        /// Необходимо для перебора из заданного множества узлов троек для построения плоскости
        /// </summary>
        private static IEnumerable<T[]> GetSubsets<T>(T[] set)
        {
            for (var i = 0; i < set.Length - 2; i++)
            {
                for (var j = i + 1; j < set.Length - 1; j++)
                {
                    for (var k = j + 1; k < set.Length; k++)
                    {
                        yield return new [] { set[i], set[j], set[k] };
                    }
                }
            }
        }

        /// <summary>
        /// Определяет, находится ли подмножество в списке используемых подмножеств
        /// </summary>
        private static bool IsSubsetUsed<T>(List<HashSet<T>> usedSubsets, HashSet<T> subset)
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