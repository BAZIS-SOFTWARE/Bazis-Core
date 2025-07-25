using Project.Interfaces.Tasks;

namespace OperationalController.Interfaces.Controller
{
    public partial interface IController
    {
        /// <summary>
        /// ChangeTaskType
        /// </summary>
        /// <param name="taskType"></param>
        void ChangeTaskType(TaskType taskType);

        /// <summary>
        /// ChangeTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        void ChangeTaskKind(TaskKind taskKind);
    }
}
