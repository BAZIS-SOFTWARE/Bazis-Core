using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Удаляет геометрический объект из модели Gmsh и соответствующие записи в внутренней структуре `ModelData`.
        /// </summary>
        /// <param name="dim">Размерность удаляемого объекта: 0 — точка, 1 — кривая, 2 — поверхность.</param>
        /// <param name="number">Тег (идентификатор) объекта в модели Gmsh.</param>
        /// <param name="recursive">
        /// Если <c>true</c>, удаление выполняется рекурсивно: вместе с дочерними элементами нижнего уровня.
        /// Если <c>false</c> (по умолчанию), удаляется только указанный элемент.
        /// </param>
        /// <remarks>
        /// Локальные функции:
        /// - `CollectDeleteTree` — собирает множество всех объектов, которые будут удалены при рекурсивном удалении.
        /// - `Collect` — рекурсивно обходит смежности lower (нижнего уровня) и заполняет множество.
        /// - `RemoveFromModelData` — выполняет фактическое удаление записи из `ModelData.ObjectData` по размерности и тегу.
        /// </remarks>
        public void DeleteGeometryObject(int dim, int number, bool recursive = false)
        {
            var (upper, _) = GmshController.Gmsh.Model.GetAdjacencies(dim, number);
            if (upper != null && upper.Length > 0)
                throw new InvalidOperationException("Удаление геометрии невозможно, так как существуют связанные элементы верхнего уровня.");

            HashSet<(int dim, int number)> removedObjects = recursive
                ? CollectDeleteTree(dim, number)
                : [(dim, number)];

            GmshController.Gmsh.Model.Occ.Remove([dim, number], recursive);
            GmshController.Gmsh.Model.Occ.Synchronize();

            foreach (var item in removedObjects)
                RemoveFromModelData(item.dim, item.number);
            PublishGeometryChange();

            HashSet<(int dim, int tag)> CollectDeleteTree(int dim, int tag)
            {
                var result = new HashSet<(int dim, int tag)>();
                Collect(dim, tag, result);
                return result;
            }

            void Collect(int dim, int tag, HashSet<(int dim, int tag)> result)
            {
                if (!result.Add((dim, tag)))
                    return;

                var (_, lower) = GmshController.Gmsh.Model.GetAdjacencies(dim, tag);

                if (lower == null || lower.Length == 0)
                    return;

                for (int i = 0; i < lower.Length; i++)
                {
                    int childDim = dim - 1;
                    int childTag = lower[i];

                    Collect(childDim, childTag, result);
                }
            }

            void RemoveFromModelData(int dim, int tag)
            {
                switch (dim)
                {
                    case 0:
                        ModelData.ObjectData.Remove(ObjType.Точка, tag);
                        break;

                    case 1:
                        ModelData.ObjectData.Remove(ObjType.Кривая, tag);
                        break;

                    case 2:
                        ModelData.ObjectData.Remove(ObjType.Поверхность, tag);
                        break;
                    case 3:
                        var volume = ModelData.VolumeData.First(vol=>vol.Number == tag);
                        ModelData.VolumeData.Remove(volume);
                        break; 
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dim));
                }
            }
        }
    }
}
