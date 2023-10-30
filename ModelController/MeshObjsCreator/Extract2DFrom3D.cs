using Model;
using ModelControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    /// <summary>
    /// Extract2DFrom3D
    /// </summary>
    public class Extract2DFrom3D : IExtractor2DFrom3D
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public IEnumerable<Element2D> Create(int StartNumber, Element3D[] elements)
        {
            var surfaces = FindOpenSurfaces(elements);

            foreach (var surface in surfaces)
            {
                var nodes = surface.GetNodes().ToArray();
                if (nodes.Length == 3)
                    yield return new Element2D(StartNumber,nodes, Model.Interfaces.ObjKind.Triangle);
                StartNumber++;
            }
        }
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Surface> FindOpenSurfaces(Element3D[] elements)
        {
            var surfDict = new Dictionary<string, List<Surface>>();

            foreach (var element in elements)
            {
                //var normals = SurfaceNormalsCoords[i];
                foreach (var surface in element.GetSurface())
                {
                    var numbers = surface.GetNodes().Select(x => x.Number).ToArray();

                    Array.Sort(numbers);
                    var surfInd = string.Join("",numbers);

                    if (!surfDict.ContainsKey(surfInd))
                        surfDict.Add(surfInd, new List<Surface>() { surface });
                    else surfDict[surfInd].Add(surface);
                }
            }

            foreach (var elemSurf in surfDict)
            {
                if (elemSurf.Value.Count == 1)
                    yield return elemSurf.Value[0];
            }

        }
    }
}
