using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace SurfaceMesher
{
    public class SaveToBPF
    {
        public static void Save(string filepath, IEnumerable<Triangle2D> triangles)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using (var writer = new StreamWriter(filepath))
            {
                var nodes = GetNodes(triangles);
                WriteBlock("Узлы", nodes, writer, n => $"{n.Index} {n.Position.X} {n.Position.Y} {0}");
                WriteBlock("Элементы2D", triangles, writer, t => $"{t.Index} {t.A.Index} {t.B.Index} {t.C.Index}");
            }
        }

        private static IEnumerable<Node2D> GetNodes(IEnumerable<Triangle2D> triangles)
        {
            var usedIndeces = new HashSet<int>();
            foreach (var triangle in triangles)
            {
                foreach(var node in triangle.GetNodes())
                {
                    if(!usedIndeces.Contains(node.Index))
                    {
                        yield return node;
                        usedIndeces.Add(node.Index);
                    }
                }
            }
        }

        private static void WriteBlock<T>(string header, IEnumerable<T> elements, StreamWriter writer, Func<T, string> formatter)
        {
            writer.WriteLine(header);
            foreach(var element in elements)
            {
                writer.WriteLine(formatter(element));
            }
            writer.WriteLine($"#{header}");
        }
    }
}
