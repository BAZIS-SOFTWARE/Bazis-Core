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

namespace OperationalController
{
    public partial class ProjectController
    {
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
            return ModelData.ObjectData.VolumeData;
        }
        /// <inheritdoc/>
        public IGroup GetModelGroup(int grIndex)
        {
            return ModelData.GroupData[grIndex];
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
        public ICondData GetCondData(int grIndex)
        {
            return TaskData[grIndex];
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
