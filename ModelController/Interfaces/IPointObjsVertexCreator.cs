using Model.Interfaces;

namespace ModelController.Interfaces
{
    public interface IPointObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject);
    }
}