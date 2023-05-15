using Geometry;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class EdgesSurfaceObjsCreator : IEdgeObjsVertexCreator
    {
        public void Create(ref int stride, bool[] vertexAtributes, ISurfaceObject[] surfaceElement)
        {
            if(surfaceElement.Length > 1)
                CreateMany(ref stride, vertexAtributes, surfaceElement);
            else
                CreateSingle(ref stride, vertexAtributes, surfaceElement);
        }

        private void CreateSingle(ref int stride, bool[] vertexAtributes, ISurfaceObject[] surfaceElement)
        {
            if(surfaceElement[0].ViewState)
                vertexAtributes[stride + 0]  = vertexAtributes[stride + 1] = vertexAtributes[stride + 2] = true;
        }

        private void CreateMany(ref int stride, bool[] vertexAtributes, ISurfaceObject[] surfaceElement)
        {
            var i = 0;
            foreach (var surfaces in surfaceElement)
            {
                if (surfaces.ViewState)
                {
                    var array = surfaces.GetSurface().ToArray();
                    for (var j = 0; j < array.Length; ++j)
                    {
                        if (array[j].ViewState)
                        {
                            var parity = (i & 1) == 0;
                            vertexAtributes[stride + 1] = true;
                            if (parity && ((array.Length > 2 && j == array.Length - 1) || (i == surfaceElement.Length - 1)))
                                vertexAtributes[stride + 0] = vertexAtributes[stride + 2] = true;
                            else if (parity)
                            {
                                vertexAtributes[stride + 0] = true;
                                vertexAtributes[stride + 2] = false;
                            }
                            else
                            {
                                vertexAtributes[stride + 0] = false;
                                vertexAtributes[stride + 2] = true;
                            }
                            stride += 3;
                            ++i;
                        }
                    }
                }
            }
        }

        private void EdgeSurfaceSorter(ref int stride, bool[] vertexAtributes, ISurfaceObject[] surfaceElement)
        {
            var surfaces = surfaceElement.SelectMany(s => s.GetSurface())
                                   .Where(s => s.ViewState)
                                   .Select((s, i) => new { s, i, c = s.CalcCentralPoint(), n = Vector.GetVectorNorm(s.CalcNormal()) })
                                   .GroupBy(k => String.Format("{0:F2}|{1:F2}|{2:F2}", k.n._x, k.n._y, k.n._z),
                                           (k, r) => r.OrderBy(s =>
                                           {
                                               //Определение основной координаты сортировки по минимальному углу между нормалью и 6 плоскостями
                                               return s.c._x;
                                           }));
                                   
                                   //.Select(r =>)
                                   //.GroupBy(,)
                                   //.Aggregate(new Point3D(0, 0, 0), (s, i) => s);
                                   //.GroupBy();
            foreach(var surface in surfaces)
            {
                
            }
        }
    }
}
