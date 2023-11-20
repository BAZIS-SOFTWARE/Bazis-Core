using Functions.Sort;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    public interface IObjectsData : IList<IModelObject>
    {
        IEnumerable<T> FindMany<T>() where T : IModelObject;

        int GetLastObjNumber();

        int NumberOfNodes { get; }

        int NumberOfVolumeElements { get; }

        int NumberOfLineElements { get; }

        int NumberOfSurfaceElements { get; }

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

        void RemoveRange(string objType);
        /// <summary>
        /// RemoveAt
        /// </summary>
        /// <param name="modelObjectInd"></param>

        void RemoveAt(int modelObjectInd);

        IModelObject Find(int objNumbr);

        List<string> GetObjectTypes();

        void AddRange(IEnumerable<IModelObject> addObjects);
    }
}