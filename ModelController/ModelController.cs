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

        public ISew3DFrom2D Sew3DFrom2D { get; } = new Sew3DFrom2D();
/// <inheritdoc/>

        public IBoundaryEdgesFinder BoundaryEdgesFinder { get; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public ISelectionHelper SelectionHelper { get; } = new SelectionHelper();
/// <inheritdoc/>

        public IFreeNodesFinder FreeNodesFinder { get; } = new FreeNodesFinder();
    }
}
