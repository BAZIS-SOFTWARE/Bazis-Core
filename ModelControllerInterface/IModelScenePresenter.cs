using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterface
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IModelScenePresenter : IDictionary<string, IObjsPresenter>
    {
        /// <summary>
        /// CreateVBOIndexes
        /// </summary>
        /// <param name="objsType"></param>
        /// <returns></returns>
        Tuple<int, int, int, int> CreateVBOIndexes(string objsType);


        /// <summary>
        /// CreateVBOVertexes
        /// </summary>
        /// <param name="objsType"></param>
        /// <param name="vertexLength"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        float[] CreateVBOVertexes(string objsType, int vertexLength, string vertexType);


        /// <summary>
        /// CreateVBOEdges
        /// </summary>
        /// <param name="objsType"></param>
        /// <param name="vertexLength"></param>
        /// <returns></returns>
        bool[] CreateVBOEdges(string objsType, int vertexLength);

        /// <summary>
        /// CreateVBOPointers
        /// </summary>
        /// <param name="objsType"></param>
        /// <param name="indexLength"></param>
        /// <returns></returns>

        int[] CreateVBOPointers(string objsType, int indexLength);

        /// <summary>
        /// HideInsideObjects
        /// </summary>
        /// <param name="objsType"></param>
        void HideInsideObjects(string objsType);
    }
}
