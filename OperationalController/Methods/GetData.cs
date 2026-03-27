using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using Model.GeometryObjects;
using MaterialDB.MaterialData;
using MaterialDB.FunctionData;
using Model.ObjectsCollections;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Data;
using System.Linq;
using Model;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetAdjacentGeometryObjects(IModelObject obj, int direction)
        {
            var dim = obj.Dim;
            int[] numbers;
            var (upperLevel, lowerLevel) = GmshController.Gmsh.Model.GetAdjacencies(dim, obj.Number);

            int dimDir;
            if (direction == 2)
            {
                dimDir = dim + 1;
                numbers = upperLevel;
            }

            else
            {
                dimDir = dim - 1;
                numbers = lowerLevel;
            }

            if (dimDir == -1)
                yield break; // очень интересное решение для возврата пустой последовательности

            else if (dimDir == 3)
                foreach (var number in numbers)
                {
                    var vol = GetModelVolumes().FirstOrDefault(x => x.Number == number);

                    foreach (var surf in vol.GetSurfaceFigures())
                        yield return surf;
                }
            else
            {
                var adgObjType = GetObjType(dimDir);

                foreach (var item in numbers)
                    yield return GetModelObject(adgObjType, item);
            }
        }

        /// <summary>
        /// Отбирает набор сущностей в пределах топологического "scope".
        /// Если <paramref name="startDim"/> и <paramref name="targetDim"/> совпадают, метод возвращает исходный набор.
        /// В противном случае сначала поднимает исходные сущности по смежностям до уровня <paramref name="targetDim"/> (см. <see cref="ClimbUp"/>),
        /// затем спускается с этого уровня обратно, чтобы собрать все сущности исходной размерности, связанные через найденные верхние элементы
        /// (см. <see cref="DescendDown"/>). В результате возвращается множество уникальных номеров сущностей размерности <paramref name="startDim"/>,
        /// которые принадлежат одному топологическому объёму при расширении до <paramref name="targetDim"/>.
        /// </summary>
        public HashSet<int> SelectByScope(int startDim, IEnumerable<int> selectedNumbers, int targetDim)
        {
            if (startDim == targetDim)
                return new HashSet<int>(selectedNumbers);

            var topLevel = TraverseAdjacenciesRecursive(startDim, selectedNumbers, targetDim);
            var result = TraverseAdjacenciesRecursive(targetDim, topLevel, startDim);

            return result;
        }
        /// <summary>
        /// Рекурсивно обходит смежности топологических сущностей, переходя от текущей размерности
        /// <paramref name="currentDim"/> к целевой <paramref name="targetDim"/>, и собирает множество
        /// уникальных номеров сущностей на целевой размерности.
        /// </summary>
        private HashSet<int> TraverseAdjacenciesRecursive(int currentDim,IEnumerable<int> currentLevel,int targetDim)
        {
            if (currentDim == targetDim)
                return new HashSet<int>(currentLevel);

            var nextLevel = new HashSet<int>();
            int step = targetDim > currentDim ? 1 : -1;

            foreach (var number in currentLevel)
            {
                var (upper, lower) = GmshController.Gmsh.Model.GetAdjacencies(currentDim, number);

                var neighbors = step > 0 ? upper : lower;

                foreach (var n in neighbors)
                    nextLevel.Add(n);
            }

            return TraverseAdjacenciesRecursive(currentDim + step, nextLevel, targetDim);
        }

        private ObjType GetObjType(int dim)
        {
            if (dim == 0)
                return ObjType.Точка;
            else if (dim == 1)
                return ObjType.Кривая;
            else
                return ObjType.Поверхность;
        }

        /// <inheritdoc/>
        public IEnumerable<Node> GetAllModelNodes()
        {
            return ModelData.ObjectData.NodesSet.Values;
        }
        /// <inheritdoc/>

        public IEnumerable<ISetInfo> GetModelSetsInfo(ObjType objType)
        {
            foreach (var item in ModelData.ObjectData.GetSetsInfo(objType))
                yield return item;

        }

        /// <inheritdoc/>
        public IEnumerable<ISetInfo> GetAllModelSetsInfo()
        {
            foreach (ObjType item in Enum.GetValues(typeof(ObjType)))
            {
                foreach (var set in ModelData.ObjectData.GetSetsInfo(item))
                    yield return set;
            }
        }



        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetModelObjects(ObjType objType)
        {
            return ModelData.ObjectData.GetObjects(objType);
        }
        /// <inheritdoc/>
        public IModelObject GetModelObject(ObjType objType, int objNumber)
        {
            return ModelData.ObjectData.Find(objType, objNumber);
        }
        /// <inheritdoc/>

        public ISetInfo GetModelSetInfo(ObjType objType, string setName)
        {
            return ModelData.ObjectData.GetSetInfo(objType, setName);
        }
/// <inheritdoc/>

        public ISetInfo GetModelSetInfo(ObjType objType, int objNumber)
        {
            return ModelData.ObjectData.GetSetInfo(objType, objNumber);
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllModelObjects()
        {
            return ModelData.ObjectData.GetAllObjects();
        }

        /// <inheritdoc/>

        public IEnumerable<IElement> GetAllModelElements()
        {
            return ModelData.ObjectData.GetAllElements();
        }

        /// <inheritdoc/>

        public IEnumerable<IElement> GetModelElements(int dim, string setName)
        {
            if (dim < 1 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");

            if (dim == 1)
                return ModelData.ObjectData.E1DCollection[setName].Values;
            else if(dim == 2)
                return ModelData.ObjectData.E2DCollection[setName].Values;
            else
                return ModelData.ObjectData.E3DCollection[setName].Values;
        }

        public IEnumerable<ISurfaceElement> GetModelSurfaceElements(int dim)
        {
            if (dim < 2 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");


            if (dim == 2)
                return ModelData.ObjectData.E2DCollection.GetObjects();
            else
                return ModelData.ObjectData.E3DCollection.GetObjects();
        }

        /// <inheritdoc/>
        public GeometryPoint GetModelPoint(int objNumber)
        {
            return ModelData.ObjectData.PointsSet[objNumber];
        }

        /// <inheritdoc/>
        public Curve GetModelCurve(int objNumber)
        {
            return ModelData.ObjectData.CurveSet[objNumber];
        }

        /// <inheritdoc/>
        public SurfaceFigure GetModelSurface(int objNumber)
        {
            return ModelData.ObjectData.SurfaceSet[objNumber];
        }

        /// <inheritdoc/>

        public IEnumerable<VolumeFigure> GetModelVolumes()
        {
            return ModelData.VolumeData;
        }

        /// <inheritdoc/>
        public MaterialDBData MaterialsDB
        {
            get { return TaskData.Materials; }
            set { TaskData.Materials = value; }
        }
        /// <inheritdoc/>

        public FunctionDBData FunctionsDB
        {
            get { return TaskData.Functions; }
            set { TaskData.Functions = value; }
        }

        /// <inheritdoc/>
        public IEnumerable<IGroup> GetAllModelGroups()
        {
            return ModelData.GroupData;
        }
        /// <inheritdoc/>
        public IEnumerable<ICondData> GetAllCondData()
        {
            return TaskData;
        }

        /// <inheritdoc/>
        public ICondData GetCondData(int condIndex)
        {
            return TaskData[condIndex];
        }
/// <inheritdoc/>

        public IGroup GetModelGroup(string grName)
        {
            return ModelData.GroupData.Find(grName);
        }
        /// <inheritdoc/>
        public IGroup GetModelGroup(int grIndex)
        {
            return ModelData.GroupData[grIndex];
        }

        /// <inheritdoc/>
        public TaskKind ProjectKind
        {
            get 
            {
                return TaskData.TaskKind;
            }
            set
            {
                TaskData.Clear();
                TaskData.TaskKind = value;
            }         
        }
        /// <inheritdoc/>
        public TaskType ProjectType
        {
            get
            {               
                return TaskData.TaskType;
            }
            set
            {
                TaskData.Clear();
                TaskData.TaskType = value;
            }
        }
    }
}
