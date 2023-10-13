using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelController
    /// </summary>
    public interface IModelController
    {
        /// <summary>
        /// CreateModelPresenter
        /// </summary>
        /// <param name="modelData"></param>
        void CreateModelPresenter(ModelData modelData);
        /// <summary>
        /// ModelPresentor
        /// </summary>
        IModelScenePresenter ModelPresentor { get;}
        /// <summary>
        /// InterfacedNodesFinder
        /// </summary>
        IInterfacedNodesFinder InterfacedNodesFinder { get; }
        /// <summary>
        /// Extractor2DFrom3D
        /// </summary>
        IExtractor2DFrom3D Extractor2DFrom3D { get; }     
    }
}
