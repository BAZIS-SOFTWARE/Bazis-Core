using Functions.Sort;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IDictionary<string, List<IModelObject>>
    {
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
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
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
        IEnumerable<IModelObject> Find(int objNumbr);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IModelObject Find(string objType, int objNumbr);
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
        [Obsolete("Старайтесь не использовать этот метод. Вместо него стандартный Add для словаря")]
        void Add(IModelObject addObj);
    }
}