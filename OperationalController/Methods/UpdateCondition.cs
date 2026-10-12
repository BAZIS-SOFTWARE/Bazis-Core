using Project.Interfaces.Tasks;
using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Заменяет условие по индексу и уведомляет об изменении списка.
        /// </summary>
        public void UpdateCondition(int index, ICondData condition)
        {
            using (BeginChanges())
            {
                if (condition == null)
                    throw new ArgumentNullException(nameof(condition));
                if (index < 0 || index >= TaskData.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                TaskData[index] = condition;
                RegisterConditionChange(condition.Kind, ChangeAction.Updated, index);
            }
        }
    }
}
