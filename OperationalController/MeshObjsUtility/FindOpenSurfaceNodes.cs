using Geometry;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace OperationalController.MeshObjsUtility
{
    public class FindOpenSurfaceNodes
    {
        public IEnumerable<int> Find(IEnumerable<IElement3D> elements)
        {
            var normalCalculator = new NormalsCalculator();
            var normals = normalCalculator.CalcNodeNormals(elements);

            foreach (var normal in normals)
            {
                if (Vector.GetVectorLength(normal.Value) > 1e-4)
                    yield return normal.Key;
            }
        }
    }
}
