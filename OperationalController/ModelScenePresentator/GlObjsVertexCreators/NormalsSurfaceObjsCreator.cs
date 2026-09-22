using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsSurfaceObjsCreator<T, U> : ISurfaceObjsVertexCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T, U> surfaceElement, System.Drawing.Color color)
        {
            foreach (var surface in surfaceElement.GetSurfaces())
            {
                var normal = surface.CalcNormal();
                for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                {
                    vertexAtributes[stride + 9 * k + 0] = normal._x;
                    vertexAtributes[stride + 9 * k + 1] = normal._y;
                    vertexAtributes[stride + 9 * k + 2] = normal._z;
                    vertexAtributes[stride + 9 * k + 3] = normal._x;
                    vertexAtributes[stride + 9 * k + 4] = normal._y;
                    vertexAtributes[stride + 9 * k + 5] = normal._z;
                    vertexAtributes[stride + 9 * k + 6] = normal._x;
                    vertexAtributes[stride + 9 * k + 7] = normal._y;
                    vertexAtributes[stride + 9 * k + 8] = normal._z;
                }

                stride += (surface.NumberOfPoints - 2) * 3 * 3;
            }
        }
    }
}
