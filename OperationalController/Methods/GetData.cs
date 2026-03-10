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
