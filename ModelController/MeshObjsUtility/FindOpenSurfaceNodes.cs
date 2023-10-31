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
        public IEnumerable<int> Find(IEnumerable<Element3D> elements)
        {
            var normalCalculator = new NormalsCalculator();
            var normals = normalCalculator.CalcNodeNormals(elements);

            foreach (var normal in normals)
            {
                if (Vector.GetVectorLenght(normal.Value) > 1e-4)
                    yield return normal.Key;
            }
        }
    }
}
