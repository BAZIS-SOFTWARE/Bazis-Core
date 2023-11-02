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

        IEnumerable<IModelObject> FindMany(ObjKind objKind);

        IEnumerable<IModelObject> FindMany(string objType);

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