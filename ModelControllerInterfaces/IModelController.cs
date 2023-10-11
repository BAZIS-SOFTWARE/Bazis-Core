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
        /// FindInterfacedNodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        Dictionary<int, List<int>>  FindInterfacedNodes(List<int> nodes, Element[] elements);
        /// <summary>
        /// FindInterfacedNodes
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        Dictionary<int, List<int>> FindInterfacedNodes(Element[] elements);
    }
}
