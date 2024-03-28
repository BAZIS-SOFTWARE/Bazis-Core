using ModelController.MeshObjsCreator;
using ModelController.MeshObjsCreator.Delone2D;
using ModelController.MeshObjsUtility;
using ModelController.ModelScenePresentator;
using ModelControllerInterfaces;
using ModelControllerInterfaces.Delone2D;
using ModelControllerInterfaces.Utilities;

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

        public ISew3DFrom2D Sew3DFrom2D { get; } = new Sew3DFrom2D();
/// <inheritdoc/>

        public IBoundaryEdgesFinder BoundaryEdgesFinder { get; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public ISelectionHelper SelectionHelper { get; } = new SelectionHelper();
/// <inheritdoc/>

        public IFreeNodesFinder FreeNodesFinder { get; } = new FreeNodesFinder();
        /// <inheritdoc/>

        public IPresentersCreator PresentersCreator { get; } = new PresentersCreator();
/// <inheritdoc/>

        public ICrossSectionMaker CrossSectionMaker { get; } = new CrossSection();
/// <inheritdoc/>

        public ICoincidentObjectsFinder CoincidentObjectsFinder { get; } = new FindCoincidentObjects();
/// <inheritdoc/>

        public IObjectsMerger ObjectsMerger { get; } = new MergeObjects();
/// <inheritdoc/>

        public IDeloneMesh2DCreator DeloneMesh2DCreator { get; } = new DeloneMesh2DCreator();
/// <inheritdoc/>

        public IMesh2DRefiner Mesh2DRefiner { get; } = new Mesh2DRefiner();
/// <inheritdoc/>

        public IObjectsRenumber ObjectsRenumber { get; } = new ObjectsRenumber();
    }
}
