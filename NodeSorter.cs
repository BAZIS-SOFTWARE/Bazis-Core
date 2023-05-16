using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class NodeSorter
    {
        /// <summary>
        /// Determines surfaces from array of nodes, which define a convex element.
        /// </summary>
        public static IEnumerable<Surface> GetSurfaces(Node[] nodes)
        {
            var surfaces = new List<HashSet<Node>>();
            var surfaceCounter = 0;
            var subsets = Get3ElementSubsets(nodes);
            foreach (var subset in subsets)
            {
                if (IsSubsetUsed(surfaces, subset.ToHashSet()))
                    continue;
                var otherNodes = nodes.ToHashSet().Except(subset);
                var points = subset.Select(x => x.Position).ToArray();
                var equationCoefficients = GetPlaneEquationCoefficients(points[0], points[1], points[2]);
                var accept = true;
                var initialSign = GetDistance(equationCoefficients, otherNodes.First().Position) >= 0;
                var potentialSurface = new List<Node>();
                foreach (var node in otherNodes)
                {
                    var distance = GetDistance(equationCoefficients, node.Position);
                    var currentSign = (distance >= 0);
                    if (Math.Abs(distance) < 1e-4)
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
                    yield return new Surface(surfaceCounter, potentialSurface.ToArray(), ObjKind.ElementSurface);
                    surfaceCounter++;
                }
            }
        }

        /// <summary>
        /// Enumerates all 3-elements subsets from source set.
        /// </summary>
        private static IEnumerable<T[]> Get3ElementSubsets<T>(T[] set)
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
        /// Computes determinant of given matrix.
        /// </summary>
        private static float GetDet(float[,] matrix)
        {
            var A = Matrix<float>.Build.DenseOfArray(matrix);
            return A.Determinant();
        }

        /// <summary>
        /// Returns a coefficients A, B, C, D of cannonical plane equation (Ax + By + Cz + D = 0) 
        /// by three given points.
        /// </summary>
        private static float[] GetPlaneEquationCoefficients(Point3D a, Point3D b, Point3D c)
        {
            var A = GetDet(new float[,] { { b._y - a._y, b._z - a._z }, 
                                          { c._y - a._y, c._z - a._z } });
            var B = GetDet(new float[,] { { b._x - a._x, b._z - a._z },
                                          { c._x - a._x, c._z - a._z } });
            var C = GetDet(new float[,] { { b._x - a._x, b._y - a._y },
                                            { c._x - a._x, c._y - a._y } });
            var D = -a._x * A + a._y * B - a._z * C;
            return new float[4] { A, -B, C, D };
        }

        /// <summary>
        /// Computes a distance from the given point to the given by coefficients plane.
        /// </summary>
        private static double GetDistance(float[] equationCoefficients, Point3D p)
        {
            var x = p._x;
            var y = p._y;
            var z = p._z;
            var A = equationCoefficients[0];
            var B = equationCoefficients[1];
            var C = equationCoefficients[2];
            var D = equationCoefficients[3];
            return (A * x + B * y + C * z + D) / Math.Sqrt(A * A + B * B + C * C);
        }

        /// <summary>
        /// Defines is subset is already used in the list of surfaces as a proper subset
        /// </summary>
        private static bool IsSubsetUsed(List<HashSet<Node>> surfaces, HashSet<Node> subset)
        {
            foreach (var surface in surfaces)
            {
                if (!subset.Except(surface).Any())
                    return true;
            }
            return false;
        }
    }
}
