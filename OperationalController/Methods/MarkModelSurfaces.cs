using Model.Utilities;
using System.Linq;

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
        private void MarkModelSurfaces()
        {
            var model = ModelData;
            if (model == null)
                return;

            var defects = surfacesMarker.Mark(model.ObjectData.E3DCollection.GetObjects());

            if (defects.Count == 0)
                return;

            var elements = string.Join(", ", defects.Select(surface => surface.ElementNumber).Distinct());
            MessageEvent?.Invoke($"Дефект сетки: грань принадлежит более чем двум элементам. Элементы: {elements}");
        }
    }
}
