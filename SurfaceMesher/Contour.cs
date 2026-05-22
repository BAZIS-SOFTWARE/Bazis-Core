using System;
using System.Collections.Generic;

namespace SurfaceMesher
{
    /// <summary>
    /// Замкнутый контур на плоскости
    /// </summary>
    public class Contour
    {
        private readonly Dictionary<Node2D, ContourElementInfo> data;

        public int NodesCount => data.Count;

        public Contour()
        {
            data = [];
        }

        public Contour(List<List<Node2D>> boundaries)
        {
            data = [];
            for(var i = 0; i < boundaries.Count; i++)
            {
                var boundary = boundaries[i];
                if(!data.TryAdd(boundary[0], new ContourElementInfo(boundary[1], boundary[boundary.Count - 1], i))
                    || !data.TryAdd(boundary[boundary.Count - 1], new ContourElementInfo(boundary[0], boundary[boundary.Count - 2], i)))
                {
                    ThrowExceptionIfNodeAlreadyExistsInContour();
                }
                for (var j = 1; j < boundary.Count - 1; j++)
                {
                    if(!data.TryAdd(boundary[j], new ContourElementInfo(boundary[j + 1], boundary[j - 1], i)))
                    {
                        ThrowExceptionIfNodeAlreadyExistsInContour();
                    }
                }
            }
        }

        public Node2D GetElementAfter(Node2D node)
        {
            return data[node].Next;
        }

        public Node2D GetElementBefore(Node2D node)
        {
            return data[node].Previous;
        }

        public int GetContourId(Node2D node)
        {
            if (!data.TryGetValue(node, out var value))
                return -1;
            return value.ContourId;
        }

        private static void ThrowExceptionIfNodeAlreadyExistsInContour()
        {
            throw new Exception("Невозможно построить триангуляцию. Задан контур с самопересечениями");
        }
    }
}