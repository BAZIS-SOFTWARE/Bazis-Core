using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;
using System.Collections.Generic;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class CoordsSurfaceObjsCreator<T, U> : ISurfaceObjsVertexCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IEnumerable<T> surfaces, System.Drawing.Color color)
        {
            foreach (var surface in surfaces)
            {
                for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                {
                    vertexAtributes[stride + 9 * k + 0] = (float)surface[0].Position._x;
                    vertexAtributes[stride + 9 * k + 1] = (float)surface[0].Position._y;
                    vertexAtributes[stride + 9 * k + 2] = (float)surface[0].Position._z;
                    vertexAtributes[stride + 9 * k + 3] = (float)surface[k + 1].Position._x;
                    vertexAtributes[stride + 9 * k + 4] = (float)surface[k + 1].Position._y;
                    vertexAtributes[stride + 9 * k + 5] = (float)surface[k + 1].Position._z;
                    vertexAtributes[stride + 9 * k + 6] = (float)surface[k + 2].Position._x;
                    vertexAtributes[stride + 9 * k + 7] = (float)surface[k + 2].Position._y;
                    vertexAtributes[stride + 9 * k + 8] = (float)surface[k + 2].Position._z;
                }

                stride += (surface.NumberOfPoints - 2) * 3 * 3;
            }
        }
    }
}
