using Geometry;
using Model.GeometryObjects;
using OperationalController.MeshObjsUtility;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace OperationalController.MeshObjsUtility
{
    /// <summary>
    /// NormalsCalculator
    /// </summary>
    public class NormalsCalculator : INormalCalculator
    {
        /// <inheritdoc/>
        public Dictionary<int, Point3D> CalcNodeNormals(IEnumerable<IElement3D> elements)
        {
            var normals = new Dictionary<int, Point3D>();
            foreach (var element in elements)
            {
                var centre = element.CalcCentr();
                foreach (var surface in element.GetSurfaces())
                {
                    var normal = surface.CalcNormal();
                    /* Эти вычисления нормали верные, но избыточны. Пока закомментируем.
                    var plane = new Plane(surface[0].Position, normal);
                    var proj = centre.GetPointProectionOnPlane(plane);
                    var normalV = proj.Sub(centre);
                    */
                    var norm = Vector.GetVectorNorm(normal);
                    foreach (var node in surface.GetVertexes())
                    {
                        if (node.ViewState)
                            if (normals.ContainsKey(node.Number))
                                normals[node.Number] = normals[node.Number].Sum(norm);
                            else normals.Add(node.Number, norm);
                    }
                }
            }

            return normals;
        }
/// <inheritdoc/>

        public IEnumerable<Curve> CalcElemsNormals(IEnumerable<ISurfaceElement> elements)
        {
            foreach (var element in elements)
            {
                foreach (var surface in element.GetSurfaces())
                {
                    var normal = surface.CalcNormal();

                    var p1 = new GeometryPoint(0, surface.CalcCentr());

                    var norm = Vector.GetVectorNorm(normal);
                    var mnorm = norm.Mult(10);
                    var p2 = new GeometryPoint(1, surface.CalcCentr().Sum(mnorm));
                    yield return new Curve(surface.Number, new GeometryPoint[] {p1,p2 });
                }
            }
        }
    }
}