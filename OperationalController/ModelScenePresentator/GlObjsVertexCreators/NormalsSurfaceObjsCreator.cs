using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;
using System.Collections.Generic;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsSurfaceObjsCreator<T, U> : ISurfaceObjsVertexCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IEnumerable<T> surfaces, System.Drawing.Color color)
        {
            foreach (var surface in surfaces)
            {
                var normal = surface.CalcNormal();
                for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                {
                    vertexAtributes[stride + 9 * k + 0] = (float)normal._x;
                    vertexAtributes[stride + 9 * k + 1] = (float)normal._y;
                    vertexAtributes[stride + 9 * k + 2] = (float)normal._z;
                    vertexAtributes[stride + 9 * k + 3] = (float)normal._x;
                    vertexAtributes[stride + 9 * k + 4] = (float)normal._y;
                    vertexAtributes[stride + 9 * k + 5] = (float)normal._z;
                    vertexAtributes[stride + 9 * k + 6] = (float)normal._x;
                    vertexAtributes[stride + 9 * k + 7] = (float)normal._y;
                    vertexAtributes[stride + 9 * k + 8] = (float)normal._z;
                }

                stride += (surface.NumberOfPoints - 2) * 3 * 3;
            }
        }
    }
}
