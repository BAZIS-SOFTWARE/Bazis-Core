using Model.Utilities;
using System.Linq;
using Model.Interfaces;
using System.IO;
using System.Collections.Generic;
using Model.Interfaces.ObjectsCollections;
using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        private readonly MarkSurfaces surfacesMarker = new MarkSurfaces();

        /// <summary>
        /// Связывает совпадающие грани объёмных элементов модели.
        /// </summary>
        /// <remarks>
        /// Вызывается после ДОБАВЛЕНИЯ элементов: загрузки, слияния, построения сетки.
        /// После удаления не нужен - IsInner проверяет ExistState соседа, поэтому
        /// погашенный элемент сам обнажает грани соседей.
        ///
        /// Обход идёт по всем наборам сразу: грань может быть общей для элементов
        /// из разных наборов, и понабортный проход оставил бы такую грань внешней.
        /// </remarks>
        private IReadOnlyCollection<ISetInfo> MarkModelSurfaces(IModelData model)
        {
            if (model == null)
                return Array.Empty<ISetInfo>();

            var modelElements = model.ObjectData.E3DCollection.GetObjects();
            var defects = surfacesMarker.Mark(modelElements, out var changedElements);

            if (defects.Count != 0)
            {
                var elements = string.Join(", ", defects.Select(surface => surface.ElementNumber).Distinct());
                throw new InvalidDataException($"Mesh face belongs to more than two elements. Elements: {elements}.");
            }

            var changedSets = new HashSet<ISetInfo>(ReferenceEqualityComparer.Instance);
            if (ReferenceEquals(model, ModelData))
                foreach (var element in changedElements)
                {
                    var setInfo = model.ObjectData.GetSetInfo(element.ObjType, element.Number);
                    if (setInfo != null)
                        changedSets.Add(setInfo);
                }
            return changedSets;
        }
    }
}
