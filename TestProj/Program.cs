using SurfaceMesher;

namespace TestProject
{
    public class Program
    {
        public static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var node1 = new Node2D(1, 0, 0);
            var node2 = new Node2D(2, 2, 0);
            var node3 = new Node2D(3, 1, 2);
            var node4 = new Node2D(4, 0, 4);
            var node5 = new Node2D(5, 2, 4);

            var contour1 = new List<Node2D>() { node1, node2, node3 };
            var contours = new List<List<Node2D>>() { contour1 };

            var nodes = new List<Node2D> { node1, node2, node3 };

            var instance = new DeloneTriangulation();
            instance.BuildTriangulation(nodes, [], contours);

            var triangles = instance.GetTriangles();

            var saver = new SaveToSTL();
            SaveToSTL.Save("./model.stl", triangles);
        }

        public static void DivideInnerEdges(HashSet<Edge> edges, List<Triangle2D> triangles, List<Node2D> nodes)
        {
            var used = new HashSet<Edge>();
            var index = nodes.Select(n => n.Index).Max();
            foreach (var triangle in triangles)
            {
                var triangleEdges = triangle.GetNodes().Bigrams(true).Select(p => new Edge(p.Item1, p.Item2));
                foreach (var edge in triangleEdges)
                {
                    if (!edges.Contains(edge) && !used.Contains(edge))
                    {
                        var middlePoint = edge.GetMiddlePoint();
                        var newNode = new Node2D(++index, middlePoint);
                        nodes.Add(newNode);
                        used.Add(edge);
                    }
                }
            }
        }

        public static IEnumerable<List<Node2D>> DivideBoundaries(List<List<Node2D>> contours, int number, List<Node2D> nodes)
        {
            var index = nodes.Select(n => n.Index).Max();
            for (var i = 0; i < contours.Count; i++)
            {
                var dividedContour = new List<Node2D>();
                foreach (var pair in contours[i].Bigrams(true))
                {
                    var vector = pair.Item2.Position.Sub(pair.Item1.Position);
                    var sequence = new List<Node2D> { pair.Item1 };
                    for (var j = 1; j < number; j++)
                    {
                        var newNode = new Node2D(++index, pair.Item1.Position.X + (double)j / number * vector.X, pair.Item1.Position.Y + (double)j / number * vector.Y);
                        nodes.Add(newNode);
                        sequence.Add(newNode);
                    }
                    dividedContour.AddRange(sequence);
                }
                yield return dividedContour;
            }
        }

        public static IEnumerable<List<Node2D>> DivideBoundaries(List<List<Node2D>> contours, double edgeSize, List<Node2D> nodes)
        {
            var index = nodes.Count;
            for (var i = 0; i < contours.Count; i++)
            {
                var dividedContour = new List<Node2D>();
                foreach (var pair in contours[i].Bigrams(true))
                {
                    var vector = pair.Item2.Position.Sub(pair.Item1.Position);
                    var length = Vector.GetVectorLength(vector);
                    var n = Math.Floor(length / edgeSize);
                    var sequence = new List<Node2D> { pair.Item1 };
                    for (var j = 1; j < n; j++)
                    {
                        var newNode = new Node2D(index++, pair.Item1.Position.X + j / n * vector.X, pair.Item1.Position.Y + j / n * vector.Y);
                        nodes.Add(newNode);
                        sequence.Add(newNode);
                    }
                    dividedContour.AddRange(sequence);
                }
                yield return dividedContour;
            }
        }

        public static (List<Node2D>, List<List<Node2D>>) ReadData(string filepath)
        {
            var mapIndexToNode = new Dictionary<int, Node2D>();
            var contours = new List<List<Node2D>>();
            using (var reader = new StreamReader(filepath))
            {
                mapIndexToNode = ReadNodes(reader).ToDictionary(n => n.Index, n => n);
                contours = ReadContours(reader, mapIndexToNode).ToList();
            }
            return (mapIndexToNode.Values.ToList(), contours);
        }

        public static IEnumerable<Node2D> ReadNodes(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length == 0)
                    yield break;
                var tokens = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                var index = int.Parse(tokens[0]);
                var x = double.Parse(tokens[1]);
                var y = double.Parse(tokens[2]);
                var node = new Node2D(index, x, y);
                yield return node;
            }
        }

        public static IEnumerable<List<Node2D>> ReadContours(StreamReader reader, Dictionary<int, Node2D> nodes)
        {
            var line = string.Empty;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line.StartsWith("//"))
                    continue;
                var tokens = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                yield return tokens.Select(s => int.Parse(s)).Select(i => nodes[i]).ToList();
            }
        }

        public static void f(List<Node2D> nodes)
        {
            var min = double.MaxValue;
            for (var i = 0; i < nodes.Count - 1; i++)
            {
                for (var j = i + 1; j < nodes.Count; j++)
                {
                    var d = Vector.GetVectorLength(nodes[j].Position.Sub(nodes[i].Position));
                    if (d < min)
                        min = d;
                }
            }
            Console.WriteLine(min);
        }
    }
}