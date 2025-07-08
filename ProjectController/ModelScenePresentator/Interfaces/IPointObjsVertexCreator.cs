using Model.Interfaces;

namespace ProjectController.ModelScenePresentator.Interfaces
{
    public interface IPointObjsVertexCreator
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="stride"></param>
        /// <param name="vertexAtributes"></param>
        /// <param name="pointObject"></param>
        void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject);
    }
}