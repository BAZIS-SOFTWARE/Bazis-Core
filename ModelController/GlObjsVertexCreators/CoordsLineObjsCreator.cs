using Model.Interfaces;
using ModelController.Interfaces;
using System.Linq;

namespace ModelController.GlObjsVertexCreators
{
    public class CoordsLineObjsCreator : ILineObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject)
        {
            if (nodeObject.ViewState)
            {
                var points = nodeObject.GetPoints().ToArray();
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