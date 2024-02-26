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
    }
}
