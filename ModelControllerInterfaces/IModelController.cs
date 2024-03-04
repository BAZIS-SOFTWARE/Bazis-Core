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
    }
}
