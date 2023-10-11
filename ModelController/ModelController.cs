using Model;
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
