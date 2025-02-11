using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// PresenterType
    /// </summary>
    public enum PresenterType
    {
        /// <summary>
        /// Point
        /// </summary>
        Point,
        /// <summary>
        /// Line
        /// </summary>
        Line,
        /// <summary>
        /// Surface
        /// </summary>
        Surface
    }
    /// <summary>
    /// IObjsPresenter
    /// </summary>
    public interface IObjsPresenter : IEnumerable<IModelObject>
    {
        /// <summary>
        /// ViewMode
        /// </summary>
        ViewMode ViewMode { get; set; }
        /// <summary>
        /// IsVolumeObjs
        /// </summary>
        bool IsVolumeObjs { get; }
        /// <summary>
        /// PresenterType
        /// </summary>
        PresenterType PresenterType { get; }
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
    }
}