using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace SurfaceMesher
{
    public class SaveToSTL
    {
        public static void Save(string filepath, IEnumerable<Triangle2D> triangles)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using var writer = new StreamWriter(filepath);
            writer.Write("solid triangle_2d_mesh\n");
            writer.Write("\n");
            foreach (var triangle in triangles)
            {
                writer.Write("facet normal 0 0 0\n");
                writer.Write("outer loop\n");
                foreach (var node in new[] { triangle.A, triangle.B, triangle.C })
                {
                    writer.Write($"vertex {node.Position.X:e5} {node.Position.Y:e5} 0\n");
                }
                writer.Write("endloop\n");
                writer.Write("endfacet\n");
                writer.Write("\n");
            }
            writer.Write("endsolid triangle_2d_mesh\n");
        }
    }
}
