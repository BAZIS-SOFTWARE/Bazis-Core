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
        public Element2D[] Create(int StartNumber, Element3D[] elements)
        {
            var surfaces = FindOpenSurfaces(elements);
            var elements2D = new Element2D[surfaces.Count];

            for (int i = 0; i < surfaces.Count; i++)
            {
                var nodes = surfaces[i].GetNodes().ToArray();
                if (nodes.Length == 3)
                    elements2D[i] = new Element2D(StartNumber,nodes, Model.Interfaces.ObjKind.Triangle);
                StartNumber++;
            }
            return elements2D;
        }
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <returns></returns>
        public List<Surface> FindOpenSurfaces(Element3D[] elements)
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

            var openSurf = new List<Surface>();

            foreach (var elemSurf in surfDict)
            {
                if (elemSurf.Value.Count == 1)
                    openSurf.Add(elemSurf.Value[0]);
            }

            return openSurf;

        }
    }
}
