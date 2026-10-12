using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class CoordsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, System.Drawing.Color color)
        {
            foreach (var point in pointObject.GetCoordinates())
            {
                vertexAtributes[stride + 0] = point._x;
                vertexAtributes[stride + 1] = point._y;
                vertexAtributes[stride + 2] = point._z;
            }
            stride += 3;
 
        }
    }
}
