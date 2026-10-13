using Model.Interfaces;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции создания и изменения групп модели.
    /// </summary>
    public interface IGroupCommands
    {
        void CreateGroup(string grName, IEnumerable<IModelObject> objs);
        void CreateGroup(IEnumerable<IModelObject> objs);
        string CreateGroupByMeshObjs(int dim, string setName);
        string CreateGroupByGeoObjs(int meshDim, int geoDim, int tag);
        string CreateOpenSurfaceNodesGroup(string setName);
        void ChangeGroupName(string oldName, string newName);
        void DeleteModelGroup(string grName);
        void ClearGroupData();
        void ClearNotExistedGroupData();
    }
}
