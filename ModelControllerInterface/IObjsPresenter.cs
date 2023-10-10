using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ModelControllerInterface
{
    /// <summary>
    /// IObjsPresenter
    /// </summary>
    public interface IObjsPresenter
    {
        /// <summary>
        /// GetObjs
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelObject> GetObjs();
        /// <summary>
        /// GetObjs
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetObjs(Color color);
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        IModelObject this[int ind] { get; }
        /// <summary>
        /// CreateIndexes
        /// </summary>
        /// <returns></returns>
        Tuple<int, int, int, int> CreateIndexes();
        /// <summary>
        /// CreatePointers
        /// </summary>
        /// <param name="indexLength"></param>
        /// <returns></returns>
        int[] CreatePointers(int indexLength);
        /// <summary>
        /// CreateVertexes
        /// </summary>
        /// <param name="vertexLength"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        float[] CreateVertexes(int vertexLength, string vertexType);
        /// <summary>
        /// CreateEdgeFlags
        /// </summary>
        /// <param name="edgesLength"></param>
        /// <returns></returns>
        bool[] CreateEdgeFlags(int edgesLength);
        /// <summary>
        /// Count
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// FindObj
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IModelObject FindObj(int objNumbr);
    }
}