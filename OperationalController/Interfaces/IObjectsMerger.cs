using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController
{
    /// <summary>
    /// IObjectsMerger
    /// </summary>
    public interface IObjectsMerger
    {
        /// <summary>
        /// Объединяет узлы и переносит связи элементов на главный узел.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers"></param>
        /// <param name="modelObjects"></param>
        /// <returns>
        /// Перечисление слитых объектов
        /// </returns>
        void Merge(List<List<int>> numbers, IObjectsSet<Node> modelObjects);

        /// <summary>
        /// Объединяет узлы и возвращает уникальные изменённые элементы и признак удаления узлов.
        /// </summary>
        void Merge(List<List<int>> numbers, IObjectsSet<Node> modelObjects, out IReadOnlyCollection<IElement> changedElements, out bool nodesRemoved);
    }
}
