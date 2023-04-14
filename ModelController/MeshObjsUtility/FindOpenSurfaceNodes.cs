using Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public class FindOpenSurfaceNodes
    {
        public IEnumerable<int> Find(Element3D[] elements)
        {
            foreach (var element in elements)
            {
                foreach (var surface in element.GetSurface())
                {
                    foreach (var node in surface.GetNodes())
                    {
                        if (node.ViewState && Vector.GetVectorLenght(node.Normal) > 1e-4)
                            yield return node.Number;
                    }
                }
            }
        }
    }
}
