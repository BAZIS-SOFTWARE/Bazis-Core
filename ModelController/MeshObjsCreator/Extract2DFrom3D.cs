using Model;
using Model.Elements;
using ModelControllerInterfaces;
using ModelInterfaces;
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
        public IEnumerable<IElement2D> Create(int StartNumber, IElement3D[] elements)
        {
            var surfaces = FindOpenSurfaces(elements);

            foreach (var surface in surfaces)
            {
                yield return new Triangle(StartNumber, surface);
                StartNumber++;
            }
        }
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISurface> FindOpenSurfaces(IElement3D[] elements)
        {
            var surfDict = new Dictionary<string, List<ISurface>>();

            foreach (var element in elements)
            {
                //var normals = SurfaceNormalsCoords[i];
                foreach (var surface in element.GetSurface())
                {
                    var numbers = surface.GetNodes().Select(x => x.Number).ToArray();

                    Array.Sort(numbers);
                    var surfInd = string.Join("",numbers);

                    if (!surfDict.ContainsKey(surfInd))
                        surfDict.Add(surfInd, new List<ISurface>() { surface });
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
