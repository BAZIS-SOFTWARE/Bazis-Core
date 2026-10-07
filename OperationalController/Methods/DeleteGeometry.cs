using Model.Interfaces;
using System;
using Model.Interfaces.ObjectsCollections;
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
            using (BeginChanges())
            {
                var (upper, _) = GmshController.Gmsh.Model.GetAdjacencies(dim, number);
                if (upper != null && upper.Length > 0)
                    throw new InvalidOperationException("Удаление геометрии невозможно, так как существуют связанные элементы верхнего уровня.");

                HashSet<(int dim, int number)> removedObjects = recursive
                    ? CollectDeleteTree(dim, number)
                    : [(dim, number)];

                GmshController.Gmsh.Model.Occ.Remove([dim, number], recursive);
                GmshController.Gmsh.Model.Occ.Synchronize();

                var changedSets = new HashSet<ISetInfo>(ReferenceEqualityComparer.Instance);
                var entityKind = GeoChange.GeometryEntityKind.None;
                foreach (var item in removedObjects)
                    RemoveFromModelData(item.dim, item.number);
                if (RemoveDeadObjects(ModelData.ObjectData.CurveSet))
                    changedSets.Add(ModelData.ObjectData.CurveSet);
                if (RemoveDeadObjects(ModelData.ObjectData.SurfaceSet))
                    changedSets.Add(ModelData.ObjectData.SurfaceSet);
                foreach (var set in changedSets)
                    RegisterSetChange(set, ChangeAction.Updated);
                ClearNotExistedGroupData();
                ModelView.Prune();
                RegisterGeometryChange(entityKind);

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
                            entityKind |= GeoChange.GeometryEntityKind.Point;
                            if (RemoveGeometryEntry(ModelData.ObjectData.PointsSet, tag))
                                changedSets.Add(ModelData.ObjectData.PointsSet);
                            break;

                        case 1:
                            entityKind |= GeoChange.GeometryEntityKind.Curve;
                            if (RemoveGeometryEntry(ModelData.ObjectData.CurveSet, tag))
                                changedSets.Add(ModelData.ObjectData.CurveSet);
                            break;

                        case 2:
                            entityKind |= GeoChange.GeometryEntityKind.Surface;
                            if (RemoveGeometryEntry(ModelData.ObjectData.SurfaceSet, tag))
                                changedSets.Add(ModelData.ObjectData.SurfaceSet);
                            break;
                        case 3:
                            entityKind |= GeoChange.GeometryEntityKind.Volume;
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
}
