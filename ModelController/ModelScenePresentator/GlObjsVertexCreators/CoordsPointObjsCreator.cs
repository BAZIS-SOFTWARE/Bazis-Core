using ModelInterfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class CoordsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject)
        {
            if(pointObject.ViewState)
            {
                foreach (var point in pointObject.GetPoints())
                {
                    vertexAtributes[stride + 0] = point._x;
                    vertexAtributes[stride + 1] = point._y;
                    vertexAtributes[stride + 2] = point._z;
                }
                stride += 3;
            }
 
        }
    }
}