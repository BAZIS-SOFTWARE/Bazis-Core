using Geometry.Exteisions;
using Model.MeshObjects;
using OperationalController.ObjsCreator;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Extract1DFrom2D
    /// </summary>
    public class Extract1DFrom2D : IExtractor1DFrom2D
    {
        /// <inheritdoc/>
        public IEnumerable<Beam> Create(int startNumber, IElement2D[] elements)
        {
            var usedElements = new HashSet<Tuple<Node, Node>>();
            foreach (var element in elements)
            {
                foreach (var edge in element.GetVertexes().Bigrams(true))
                {
                    var inversedEdge = Tuple.Create(edge.Item2, edge.Item1);
                    if (!usedElements.Contains(edge) && !usedElements.Contains(inversedEdge))
                        usedElements.Add(edge);
                    else if(usedElements.Contains(edge))
                        usedElements.Remove(edge);
                    else if(usedElements.Contains(inversedEdge))
                        usedElements.Remove(inversedEdge);
                }
            }
            return usedElements.Select((t, i) => new Beam(startNumber + i, new[] { t.Item1, t.Item2 }));
        }
    }
}
