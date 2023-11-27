using Functions.Sort;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IDictionary<string, List<IModelObject>>
    {
        /// <summary>
        /// FindMany
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> FindMany<T>() where T : IModelObject;
        /// <summary>
        /// objType
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        int GetLastObjNumber(string objType);
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
        /// FindMany
        /// </summary>
        /// <param name="objKind"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> FindMany(ObjKind objKind);
        /// <summary>
        /// FindMany
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> FindMany(string objType);
        /// <summary>
        /// ClearRemoved
        /// </summary>
        void ClearRemoved();
        /// <summary>
        /// RemoveAt
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objInd"></param>
        void RemoveAt(string objType,int objInd);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IModelObject Find(int objNumbr);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="addObjects"></param>
        void AddRange(IEnumerable<IModelObject> addObjects);
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="addObj"></param>
        void Add(IModelObject addObj);
    }
}