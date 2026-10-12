using System;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// IProjectLoader
    /// </summary>
    public interface IProjectLoader
    {
        /// <summary>
        /// Загружает данные и сообщает условный прогресс в процентах.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="progress">Получатель условного прогресса в процентах.</param>
        /// <returns></returns>
        IProjectData Load(string fullPath, IProgress<int> progress = null);
    }
}