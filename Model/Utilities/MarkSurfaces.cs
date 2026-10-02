using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;

namespace Model.Utilities
{
    /// <summary>
    /// Связывает совпадающие поверхности объёмных элементов.
    /// После связывания поверхность знает своего соседа, а IsInner считается по нему.
    /// </summary>
    /// <remarks>
    /// Вызывается после загрузки модели и после ДОБАВЛЕНИЯ элементов.
    /// После удаления вызывать не нужно: IsInner проверяет ExistState соседа,
    /// поэтому погашенный элемент сам обнажает поверхности соседей.
    /// </remarks>
    public class MarkSurfaces
    {
        /// <summary>
        /// Расставляет соседей у поверхностей переданных элементов.
        /// </summary>
        /// <param name="elements">Элементы, чьи поверхности связываются между собой</param>
        /// <returns>
        /// Поверхности, встреченные более двух раз. Это дефект сетки: у грани
        /// не может быть трёх владельцев. Список пуст, если сетка корректна.
        /// </returns>
        /// <remarks>
        /// Результат собирается сразу, а не отложенно: метод меняет состояние
        /// поверхностей, и ленивое перечисление откладывало бы саму работу.
        /// </remarks>
        public IReadOnlyList<ElementSurface> Mark(IEnumerable<ISurfaceElement> elements)
        {
            return Mark(elements, out _);
        }

        /// <summary>
        /// Связывает поверхности и возвращает уникальные элементы с изменёнными соседями.
        /// </summary>
        public IReadOnlyList<ElementSurface> Mark(IEnumerable<ISurfaceElement> elements, out IReadOnlyCollection<IElement> changedElements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            var found = new Dictionary<int[], ElementSurface>(new SurfaceNodesComparer());
            var defects = new List<ElementSurface>();
            var changed = new HashSet<IElement>(ReferenceEqualityComparer.Instance);
            changedElements = changed;

            foreach (var element in elements)
            {
                if (!element.ExistState)
                    continue;

                foreach (var surface in element.GetSurfaces())
                    Link(surface, found, defects, changed);
            }

            foreach (var surface in found.Values)
                if (surface != null && surface.SetNeighbour(null))
                    changed.Add(surface.Owner);

            return defects;
        }

        /// <summary>
        /// Обрабатывает одну поверхность: запоминает первую встреченную,
        /// связывает вторую с первой, третью считает дефектом.
        /// </summary>
        private void Link(ElementSurface surface, Dictionary<int[], ElementSurface> found, List<ElementSurface> defects, ISet<IElement> changed)
        {
            var key = surface.CreateNodesKey();
            var opposite = default(ElementSurface);

            if (!found.TryGetValue(key, out opposite))
            {
                found[key] = surface;
                return;
            }

            if (opposite == null)
            {
                if (surface.SetNeighbour(null))
                    changed.Add(surface.Owner);
                defects.Add(surface);
                return;
            }

            if (opposite.SetNeighbour(surface.Owner))
                changed.Add(opposite.Owner);
            if (surface.SetNeighbour(opposite.Owner))
                changed.Add(surface.Owner);

            // Пара закрыта. Пустая запись отличает "встретилась дважды"
            // от "встретилась один раз" и ловит третьего владельца.
            found[key] = null;
        }
    }
}
