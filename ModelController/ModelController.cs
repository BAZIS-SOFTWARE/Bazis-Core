using ModelController.MeshObjsCreator;
using ModelController.MeshObjsUtility;
using ModelController.ModelScenePresentator;
using ModelControllerInterfaces;
using ModelControllerInterfaces.SurfaceMeshCreator;
using ModelControllerInterfaces.MeshObjsUtility;
using ModelControllerInterfaces.Utilities;
using ModelControllerInterfaces.ObjsCreator;
using Model.Interfaces;

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

        public IBoundaryEdgesFinder BoundaryEdgesFinder { get; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public ISelectionHelper SelectionHelper { get; } = new SelectionHelper();
/// <inheritdoc/>

        public IFreeNodesFinder FreeNodesFinder { get; } = new FreeNodesFinder();

/// <inheritdoc/>

        public ICrossSectionMaker CrossSectionMaker { get; } = new CrossSection();
/// <inheritdoc/>

        public ICoincidentObjectsFinder CoincidentObjectsFinder { get; } = new FindCoincidentObjects();
/// <inheritdoc/>

        public IObjectsMerger ObjectsMerger { get; } = new MergeObjects();
/// <inheritdoc/>

        public ISurfaceMeshCreator SurfaceMeshCreator { get; set; }

/// <inheritdoc/>

        public IObjectsRenumber ObjectsRenumber { get; } = new ObjectsRenumber();
/// <inheritdoc/>

        public INormalCalculator NormalCalculator { get; } = new NormalsCalculator();
/// <inheritdoc/>

        public IVolumeMeshCreator VolumeMeshCreator { get; set; }

/// <inheritdoc/>

        public IExtractor1DFrom2D Extractor1DFrom2D { get; } = new Extract1DFrom2D();     
    }
}
