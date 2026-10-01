using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции создания и изменения условий задачи.
    /// </summary>
    public interface IConditionCommands
    {
        void AddTaskData(ICondData cond);
        ICondData Create(DataKind kind, string info);
        /// <summary>Заменяет условие и сообщает об изменении по его индексу.</summary>
        void UpdateCondition(int index, ICondData condition);
        void DeleteCond(IGroup group);
        bool DeleteCond(int index);
        void ClearTaskData();
        void ClearNotExistedCondData();
    }
}
