using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Drawing;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class LayoutLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject)
        {
            if (nodeObject.ViewState)
            {
                var points = nodeObject.GetCoordinates().ToArray();
                for (int k = 0; k < points.Length; k++)
                {
                    vertexAtributes[stride + k] = nodeObject.Number;
                }
                stride += nodeObject.NumberOfPoints;
            }
        }
    }
}
