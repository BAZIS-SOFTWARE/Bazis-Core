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
        public IModelScenePresenter ModelPresentor { get; internal set; } 
/// <inheritdoc/>

        public void CreateModelPresenter(ModelData modelData)
        {
            ModelPresentor = new ModelScenePresentator.ModelScenePresentator(modelData);
        }
/// <inheritdoc/>

        public Element2D[] Extract2DFrom3D(int startInd, Element3D[] elements)
        {
            var extractor = new Extract2DFrom3D(elements);
            return extractor.Create(startInd);
        }

        /// <inheritdoc/>

        public Dictionary<int, List<int>> FindInterfacedNodes(List<int> nodes, Element[] elements)
        {
            var finder = new InterfacedNodesFinder(elements);
            return finder.Find(nodes);
        }
/// <inheritdoc/>

        public Dictionary<int, List<int>> FindInterfacedNodes(Element[] elements)
        {
            var finder = new InterfacedNodesFinder(elements);
            return finder.Find();
        }
    }
}
