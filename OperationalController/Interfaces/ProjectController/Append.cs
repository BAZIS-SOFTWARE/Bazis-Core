using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Загружает модель из файла и добавляет её объекты и группы в текущую модель.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// Имена наборов, возвращённые <see cref="Model.Interfaces.IModelData.Append"/>.
        /// </returns>
        IReadOnlyList<string> Append(string path);


    }
}
