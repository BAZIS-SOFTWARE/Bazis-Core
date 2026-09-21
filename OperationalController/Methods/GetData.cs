using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
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
        public TaskKind TaskKind
        {
            get { return TaskData.TaskKind; }
            //set { TaskData.Clear(); TaskData.TaskKind = value; }
        }
        /// <inheritdoc/>
        public TaskType TaskType
        {
            get { return TaskData.TaskType; }
            //set { TaskData.Clear(); TaskData.TaskType = value; }
        }

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
        /// В противном случае сначала поднимает исходные сущности по смежностям до уровня <paramref name="targetDim"/>
        /// затем спускается с этого уровня обратно, чтобы собрать все сущности исходной размерности, связанные через найденные верхние элементы
        /// В результате возвращается множество уникальных номеров сущностей размерности <paramref name="startDim"/>,
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
        public IEnumerable<ISetInfo> GetModelSetsInfo(ObjType objType)
        {
            foreach (var item in ModelData.ObjectData.GetSetsInfo(objType))
                yield return item;
        }

        /// <inheritdoc/>
        public IEnumerable<ISetInfo> GetAllModelSetsInfo()
        {
            foreach (ObjType item in Enum.GetValues(typeof(ObjType)))
                foreach (var set in ModelData.ObjectData.GetSetsInfo(item))
                    yield return set;
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
        public (double X, double Y, double Z) GetCenterOfMass(int dim, int tag)
        {
            var center = GmshController.Gmsh.Model.Occ.GetCenterOfMass(dim, tag);

            return (center.Item1, center.Item2, center.Item3);
        }

        /// <summary>
        /// Возвращает кортеж массивов номеров геометрических объектов, смежных с указанным объектом.
        /// Первый элемент кортежа содержит номера смежных объектов более высокой размерности (upper),
        /// второй — номера смежных объектов более низкой размерности (lower).
        /// </summary>
        /// <param name="dim">Размерность исходного объекта (0 — точка, 1 — кривая, 2 — поверхность и т.д.).</param>
        /// <param name="number">Топологический номер (tag) исходного объекта в модели.</param>
        /// <returns>
        /// Кортеж из двух массивов: (upper[], lower[]). Массивы могут быть пустыми, если смежностей нет.
        /// </returns>
        public Tuple<int[], int[]> GetAdjacentGeometryObjects(int dim, int number) => GmshController.Gmsh.Model.GetAdjacencies(dim, number);

        public IEnumerable<Node> GetAllModelNodes() => ModelData.ObjectData.NodesSet.Values;
        public GeometryPoint GetModelPoint(int objNumber) => ModelData.ObjectData.PointsSet[objNumber];
        public Curve GetModelCurve(int objNumber) => ModelData.ObjectData.CurveSet[objNumber];
        public SurfaceFigure GetModelSurface(int objNumber) => ModelData.ObjectData.SurfaceSet[objNumber];
        public IEnumerable<VolumeFigure> GetModelVolumes() => ModelData.VolumeData;
        public IEnumerable<IGroup> GetAllModelGroups() => ModelData.GroupData;
        public IEnumerable<ICondData> GetAllCondData() => TaskData;
        public ICondData GetCondData(int condIndex) => TaskData[condIndex];
        public IGroup GetModelGroup(string grName) => ModelData.GroupData.Find(grName);
        public IGroup GetModelGroup(int grIndex) => ModelData.GroupData[grIndex];
        public IEnumerable<IModelObject> GetModelObjects(ObjType objType) => ModelData.ObjectData.GetObjects(objType);
        public IModelObject GetModelObject(ObjType objType, int objNumber) => ModelData.ObjectData.Find(objType, objNumber);
        /// <summary>
        /// Возвращает информацию о множестве (set) объектов указанного типа по имени множества.
        /// </summary>
        /// <param name="objType">Тип объектов множества.</param>
        /// <param name="setName">Имя множества.</param>
        /// <returns>Информация о множестве в виде <see cref="ISetInfo"/>.</returns>
        public ISetInfo GetModelSetInfo(ObjType objType, string setName) => ModelData.ObjectData.GetSetInfo(objType, setName);
        /// <summary>
        /// Возвращает информацию о множестве, содержащем конкретный объект (по номеру).
        /// </summary>
        /// <param name="objType">Тип объекта.</param>
        /// <param name="objNumber">Номер (tag) объекта.</param>
        /// <returns>Информация о множестве (<see cref="ISetInfo"/>), которому принадлежит объект.</returns>
        public ISetInfo GetModelSetInfo(ObjType objType, int objNumber) => ModelData.ObjectData.GetSetInfo(objType, objNumber);
        public IEnumerable<IModelObject> GetAllModelObjects() => ModelData.ObjectData.GetAllObjects();
        public IEnumerable<IElement> GetAllModelElements() => ModelData.ObjectData.GetAllElements();
        public double[] GetGeoObjPoints(int dim, int tag, double[] parametricCoord) => GmshController.Gmsh.Model.GetValue(dim, tag, parametricCoord);
        /// <summary>
        /// Возвращает параметрические координаты геометрического объекта для заданной точки.
        /// </summary>
        public double[] GetParametrization(int dim, int tag, double[] coordinates) => GmshController.Gmsh.Model.GetParametrization(dim, tag, coordinates);
        public Tuple<double[], double[]> GetParametrizationBounds(int dim, int tag) => GmshController.Gmsh.Model.GetParametrizationBounds(dim, tag);
        /// <summary>
        /// Возвращает нормаль поверхности в заданных параметрических координатах.
        /// </summary>
        public double[] GetSurfaceNormal(int surfaceTag, double[] parametricCoordinates) => GmshController.Gmsh.Model.GetNormal(surfaceTag, parametricCoordinates);
        public string GetType(int dim, int tag) => GmshController.Gmsh.Model.GetType(dim, tag);
        public int[] GetBoundary(int[] dimTags, bool combined = true, bool oriented = true, bool recursive = false) => GmshController.Gmsh.Model.GetBoundary(dimTags, combined, oriented, recursive);
        public double GetCurveLength(int curveTag) => GmshController.CalcCurveLength(curveTag);
    }
}
