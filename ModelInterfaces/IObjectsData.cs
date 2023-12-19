using Functions.Sort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IEnumerable<KeyValuePair<ObjType, List<IModelObject>>>
    {
        /// <summary>
        /// objType
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IModelObject GetLastObj(ObjType objType);
        /// <summary>
        /// Count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIndex"></param>
        /// <returns></returns>
        IModelObject this[ObjType objType, int objIndex] { get;set; }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        List<IModelObject> this[ObjType objType] { get;}
        /// <summary>
        /// Clear
        /// </summary>
        void Clear();
        /// <summary>
        /// ContainsKey
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        bool ContainsKey(ObjType objType);
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
        /// <param name="objType"></param>
        /// <param name="addObj"></param>
        void Add(ObjType objType,IModelObject addObj);

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="addObjs"></param>
        void Add(ObjType objType, IEnumerable<IModelObject> addObjs);
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="objType"></param>
        void Remove(ObjType objType);
    }
}