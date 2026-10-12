using System;
using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// ITaskDataLoader
    /// </summary>
    public interface ITaskDataLoader
    {
        /// <summary>
        /// Загружает данные и сообщает условный прогресс в процентах.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupData"></param>
        /// <param name="progress">Получатель условного прогресса в процентах.</param>
        /// <returns></returns>
        ITaskData Load(string path, IGroupData groupData, IProgress<int> progress = null);
    }
}
