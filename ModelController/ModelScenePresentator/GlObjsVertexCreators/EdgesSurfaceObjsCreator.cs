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
            foreach (var surfaces in surfaceElement)
            {
                if (surfaces.ViewState)
                {
                    var surfacesCount = 0;
                    foreach (var surface in surfaces.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            if ((surfacesCount & 1) == 0)
                            {
                                vertexAtributes[stride + 0] = true;
                                vertexAtributes[stride + 1] = true;
                                vertexAtributes[stride + 2] = false;
                            }
                            else
                            {
                                vertexAtributes[stride + 0] = false;
                                vertexAtributes[stride + 1] = true;
                                vertexAtributes[stride + 2] = true;
                            }
                            surfacesCount++;
                            stride += 3;
                        } 
                    }
                }
            }
        }
    }
}
