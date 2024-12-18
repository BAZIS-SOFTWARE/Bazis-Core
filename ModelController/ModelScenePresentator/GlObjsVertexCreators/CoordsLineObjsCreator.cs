using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class CoordsLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject)
        {
            if (nodeObject.ViewState)
            {
                var points = nodeObject.GetCoordinates().ToArray();
                for (int k = 0; k < points.Length; k++)
                {
                    vertexAtributes[stride + 3 * k + 0] = points[k]._x;
                    vertexAtributes[stride + 3 * k + 1] = points[k]._y;
                    vertexAtributes[stride + 3 * k + 2] = points[k]._z;
                }
                stride += nodeObject.NumberOfPoints * 3;
            }
        }
    }
}