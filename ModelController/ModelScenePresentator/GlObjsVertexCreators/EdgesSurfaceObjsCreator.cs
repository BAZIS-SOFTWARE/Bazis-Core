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
    public class EdgesSurfaceObjsCreator
    {
        public void Create(ref int stride, bool[] vertexAtributes, ISurfaceObject[] surfaceElements)
        {
            foreach (var surfaceElement in surfaceElements)
            {
                if (surfaceElement.ViewState)
                {
                    foreach (var surface in surfaceElement.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            for (int i = 0; i < surface.NumberOfPoints - 2; i++)
                            {
                                if (i == 0)
                                {
                                    vertexAtributes[stride + 3 * i] = true;
                                    vertexAtributes[stride + 3 * i + 1] = true;
                                    vertexAtributes[stride + 3 * i + 2] = true;
                                }
 
                                else
                                {
                                    vertexAtributes[stride + 3 * i - 1] = false;
                                    vertexAtributes[stride + 3 * i] = false;
                                    vertexAtributes[stride + 3 * i + 1] = true;
                                    vertexAtributes[stride + 3 * i + 2] = true;
                                }
                            }
                            stride += (surface.NumberOfPoints - 2) * 3;
                        }
                    }
                }
            }
        }
    }
}
