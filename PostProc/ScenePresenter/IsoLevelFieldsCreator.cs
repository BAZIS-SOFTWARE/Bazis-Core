using Geometry;
using ModelInterfaces.MeshObjects;
using ProjectInterfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Results.ScenePresenter
{
    class IsoLevelsCreator
    {
        public IsoLevelsCreator()
        {
            //CreateInSidePoligons
        }

        public Dictionary<Color, Dictionary<int, List<Point3D>>> CreateInSidePoligons(IResult results, string resultType, ISurfaceElement[] elements, List<float[]> values, Color[] colors)
        {
            var inSidePoligons = new Dictionary<Color, Dictionary<int, List<Point3D>>>();

            foreach (var element in elements)
            {
                if (element.ViewState)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        var color = colors[i];
                        var val = values[i][0];

                        var nodesResults = new List<float>();
                        foreach (var node in element.GetVertexes())
                        {
                            nodesResults.Add(results.GetNodeValue(node.Number, resultType));
                        }

                        var points = FindCrossElemCoords(val, element, nodesResults.ToArray());

                        //if (!inSidePoligons.Keys.Contains(color, new ColorEqualityComparer()))
                        if (!inSidePoligons.Keys.Contains(color))
                            inSidePoligons.Add(color, new Dictionary<int, List<Point3D>>());

                        inSidePoligons[color].Add(element.Number, points);
                    }
                }
            }
            return inSidePoligons;
        }

        public static List<Point3D> FindCrossElemCoords(float value, ISurfaceElement element, float[] nodesValues)
        {
            var points = new List<Point3D>();

            foreach (var surface in element.GetSurfaces())
            {
                var nodesOnSide = surface.GetVertexes().ToArray();
                for (int j = 0; j < nodesOnSide.Length; j++)
                {
                    //var fObjPtr = nodesOnSide[j];
                    for (int k = j + 1; k < nodesOnSide.Length; k++)
                    {
                        //var sObjPtr = element.ObjPtr[nodesOnSide * i + k];

                        var p1V = nodesValues[j];
                        var p2V = nodesValues[k];
                        var p1 = nodesOnSide[j].Position;
                        var p2 = nodesOnSide[k].Position;

                        float[] ndRes;
                        Point3D[] ndCoords;

                        SortValues.Sort(p1, p2, p1V, p2V, out ndRes, out ndCoords);

                        if (value >= ndRes[0] & value <= ndRes[1])
                        {
                            var line = new Segment3D(p1, p2);
                            var newPoint = line.GetPointOnLine(p1V, p2V, value);
                            //var newPoint = Geometry.GetPointOnLine(p1, p2, p1V, p2V, value);
                            if (!points.Contains(newPoint, new Point3DEqualityComparer()))
                                points.Add(newPoint);
                        }

                    }
                }
            }
            return points;
        }
    }
}
