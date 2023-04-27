using Model.Interfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface IPointObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject);
    }
}