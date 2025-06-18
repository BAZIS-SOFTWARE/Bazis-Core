using ModelControllerInterfaces.SurfaceMeshCreator;
using ModelControllerInterfaces.MeshObjsUtility;
using ModelControllerInterfaces.ObjsCreator;
using Model.Interfaces;
using ModelController.MeshObjsUtility;

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
        /// Extractor1DFrom2D
        /// </summary>
        IExtractor1DFrom2D Extractor1DFrom2D { get; }
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
        ISurfaceMeshCreator SurfaceMeshCreator { get; }
        /// <summary>
        /// VolumeMesh3DCreator
        /// </summary>
        IVolumeMeshCreator VolumeMeshCreator { get; }
        /// <summary>
        /// NormalCalculator
        /// </summary>
        INormalCalculator NormalCalculator { get; }
    }
}
