using Functions.Sort;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IList<IModelObject>
    {
        /// <summary>
        /// FindMany
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> FindMany<T>() where T : IModelObject;
        /// <summary>
        /// Sort objects by number use merge sort
        /// </summary>
        void Sort();
        /// <summary>
        /// GetLastObjNumber
        /// </summary>
        /// <returns></returns>
        int GetLastObjNumber();
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
        /// Remove a range of objects. It is advisable to do a sort after removing.
        /// </summary>
        /// <param name="objType"></param>
        void RemoveRange(string objType);
        /// <summary>
        /// RemoveAt
        /// </summary>
        /// <param name="modelObjectInd"></param>

        void RemoveAt(int modelObjectInd);

        IModelObject Find(int objNumbr);
        /// <summary>
        /// Get objects types
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetObjectTypes();

        void AddRange(IEnumerable<IModelObject> addObjects);
    }
}