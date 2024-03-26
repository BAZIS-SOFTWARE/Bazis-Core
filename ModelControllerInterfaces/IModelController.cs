using ModelControllerInterfaces.Delone2D;
using ModelControllerInterfaces.Utilities;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelController
    /// </summary>
    public interface IModelController
    {
        /// <summary>
        /// InterfacedNodesFinder
        /// </summary>
        IObjectsRenumber ObjectsRenumber { get; }

        /// <summary>
        /// InterfacedNodesFinder
        /// </summary>
        IInterfacedNodesFinder InterfacedNodesFinder { get; }
        /// <summary>
        /// Extractor2DFrom3D
        /// </summary>
        IExtractor2DFrom3D Extractor2DFrom3D { get; }
        /// <summary>
        /// Sew3DFrom2D
        /// </summary>
        ISew3DFrom2D Sew3DFrom2D { get; }
        /// <summary>
        /// BoundaryEdgesFinder
        /// </summary>
        IBoundaryEdgesFinder BoundaryEdgesFinder { get; }

        /// <summary>
        /// SelectionHelper
        /// </summary>
        ISelectionHelper SelectionHelper { get; }

        /// <summary>
        /// FreeNodesFinder
        /// </summary>
        IFreeNodesFinder FreeNodesFinder { get; }

        /// <summary>
        /// PresentersCreator
        /// </summary>
        IPresentersCreator PresentersCreator { get; }
        /// <summary>
        /// CrossSectionMaker
        /// </summary>
        ICrossSectionMaker CrossSectionMaker { get; }

        /// <summary>
        /// CoincidentObjectsFinder
        /// </summary>
        ICoincidentObjectsFinder CoincidentObjectsFinder { get; }

        /// <summary>
        /// ObjectsMerger
        /// </summary>
        IObjectsMerger ObjectsMerger { get; }
        /// <summary>
        /// DeloneMesh2DCreator. Creates 2D mesh use already existed nodes by Delone criterion. 
        /// </summary>
        IDeloneMesh2DCreator DeloneMesh2DCreator { get; }

        /// <summary>
        /// IMesh2DRefiner
        /// </summary>
        IMesh2DRefiner Mesh2DRefiner { get; }
    }
}
