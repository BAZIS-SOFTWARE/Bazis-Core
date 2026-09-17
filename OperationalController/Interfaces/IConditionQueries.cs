using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции чтения условий задачи.
    /// </summary>
    public interface IConditionQueries
    {
        IEnumerable<ICondData> GetAllCondData();
        ICondData GetCondData(int condIndex);
    }
}
