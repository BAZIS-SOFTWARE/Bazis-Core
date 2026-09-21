using Model.Interfaces;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции чтения групп модели.
    /// </summary>
    public interface IGroupQueries
    {
        IEnumerable<IGroup> GetAllModelGroups();
        IGroup GetModelGroup(int grIndex);
        IGroup GetModelGroup(string grName);
        bool ContainsConditions(string grName);
    }
}
