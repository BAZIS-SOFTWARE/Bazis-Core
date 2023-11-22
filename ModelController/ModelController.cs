using Model;
using ModelController.MeshObjsCreator;
using ModelController.MeshObjsUtility;
using ModelControllerInterfaces;
using System.Collections.Generic;

namespace ModelController
{
    /// <summary>
    /// ModelController
    /// </summary>
    public class ModelController : IModelController
    {
/// <inheritdoc/>

        public IInterfacedNodesFinder InterfacedNodesFinder { get; } = new InterfacedNodesFinder();
/// <inheritdoc/>

        public IExtractor2DFrom3D Extractor2DFrom3D { get; } = new Extract2DFrom3D();

        /// <inheritdoc/>

        public void CreateModelPresenter(ModelData modelData)
        {
            ModelPresentor = new ModelScenePresentator.ModelScenePresentator(modelData.ObjectData);
        }

    }
}
