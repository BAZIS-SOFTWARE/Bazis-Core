using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class CoordsSurfaceObjsCreator<T, U> : ISurfaceObjsVertexCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T, U> surfaceElement, IModelView modelView)
        {
            foreach (var surface in surfaceElement.GetSurfaces())
            {
                for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                {
                    vertexAtributes[stride + 9 * k + 0] = surface[0].Position._x;
                    vertexAtributes[stride + 9 * k + 1] = surface[0].Position._y;
                    vertexAtributes[stride + 9 * k + 2] = surface[0].Position._z;
                    vertexAtributes[stride + 9 * k + 3] = surface[k + 1].Position._x;
                    vertexAtributes[stride + 9 * k + 4] = surface[k + 1].Position._y;
                    vertexAtributes[stride + 9 * k + 5] = surface[k + 1].Position._z;
                    vertexAtributes[stride + 9 * k + 6] = surface[k + 2].Position._x;
                    vertexAtributes[stride + 9 * k + 7] = surface[k + 2].Position._y;
                    vertexAtributes[stride + 9 * k + 8] = surface[k + 2].Position._z;
                }

                stride += (surface.NumberOfPoints - 2) * 3 * 3;
            }
        }
    }
}
