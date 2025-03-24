using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class LayoutPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject)
        {
            if (pointObject.ViewState)
            {
                foreach (var point in pointObject.GetCoordinates())
                {
                    vertexAtributes[stride] = pointObject.Number;
                }
                stride += 1;
            }
        }
    }
}
