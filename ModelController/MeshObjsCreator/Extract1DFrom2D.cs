using ExtensionsLib;
using Geometry;
using Model.MeshObjects;
using ModelControllerInterfaces.ObjsCreator;
using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    public class Extract1DFrom2D : IExtractor1DFrom2D
    {
        public IEnumerable<ILineObject<INode>> Create(int StartNumber, IElement2D[] elements)
        {
            var segments = FindOpenSegments(elements);
            return segments.Select((p, i) => new Beam(StartNumber + i, new[] { p.Item1, p.Item2 }));
        }

        public IEnumerable<(INode, INode)> FindOpenSegments(IElement2D[] elements)
        {
            var usedElements = new HashSet<Tuple<INode, INode>>();
            foreach(var element in elements)
            {
                foreach(var edge in element.GetVertexes().Bigrams(true))
                {
                    var inversedEdge = Tuple.Create(edge.Item2, edge.Item1);
                    if (!usedElements.Contains(edge) && !usedElements.Contains(inversedEdge))
                        usedElements.Add(edge);
                    else
                        usedElements.Remove(edge);
                }
            }
            return usedElements.Select(t => (t.Item1, t.Item2));
        }
    }
}
