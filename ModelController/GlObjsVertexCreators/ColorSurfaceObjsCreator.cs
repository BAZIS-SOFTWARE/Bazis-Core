using Model.Interfaces;
using ModelController.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.GlObjsVertexCreators
{
    public class ColorSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject element)
        {
            foreach (var surface in element.GetSurface())
            {
                if (surface.ViewState)
                {
                    for (int k = 0; k < surface.NumberOfPoints; k++)
                    {
                        vertexAtributes[stride + 3 * k + 0] = element.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 3 * k + 1] = element.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 3 * k + 2] = element.MasterColor.B / 255.0f;
                    }
                    stride += surface.NumberOfPoints * 3;
                }
                
            }
         }
    }
}
