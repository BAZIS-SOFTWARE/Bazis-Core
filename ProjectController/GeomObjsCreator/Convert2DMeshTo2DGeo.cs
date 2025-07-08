using Model.GeometryObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectController.GeomObjsCreator
{
    public class Convert2DMeshTo2DGeo
    {
        IEnumerable<GeometrySurface> Convert(IEnumerable<Element2D> elements)
        {
            foreach (var item in elements)
            {
                var geoPoints = new List<GeometryPoint>();
                foreach (var vert in item.GetVertexes())
                    geoPoints.Add(new GeometryPoint(vert.Number, vert.Position));
                yield return new GeometrySurface(item.Number, geoPoints.ToArray());
            }
        }
    }
}
