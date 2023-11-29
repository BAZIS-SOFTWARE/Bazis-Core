using Functions.Sort;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IDictionary<ObjType, List<IModelObject>>
    {
        /// <summary>
        /// objType
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IModelObject GetLastObj(ObjType objType);
        /// <summary>
        /// NumberOfNodes
        /// </summary>
        int NumberOfNodes { get; }
        /// <summary>
        /// NumberOfVolumeElements
        /// </summary>
        int NumberOfVolumeElements { get; }
        /// <summary>
        /// NumberOfLineElements
        /// </summary>
        int NumberOfLineElements { get; }
        /// <summary>
        /// NumberOfSurfaceElements
        /// </summary>
        int NumberOfSurfaceElements { get; }
        /// <summary>
        /// NumberOfElements
        /// </summary>
        int NumberOfElements { get; }
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
        /// <summary>
        /// RemoveAt
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objInd"></param>
        void RemoveAt(ObjType objType,int objInd);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> FindMany(int objNumbr);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IModelObject Find(ObjType objType, int objNumbr);
        /// <summary>
        /// FindMany
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> FindMany<T>() where T : IModelObject;
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="addObj"></param>
        void Add(IModelObject addObj);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="addObjs"></param>
        void AddRange(IEnumerable<IModelObject> addObjs);
    }
}