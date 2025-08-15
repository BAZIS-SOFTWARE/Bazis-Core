using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public IEnumerable<ISetInfo> GetModelSetsInfo(ObjType objType)
        {
            foreach (var item in ModelData.ObjectData.GetSetsInfo(objType))
                yield return item;

        }
        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetModelObjects(ObjType objType)
        {
            return ModelData.ObjectData.GetObjects(objType);
        }
        /// <inheritdoc/>
        public IModelObject GetModelObject(ObjType objType, int setNumber)
        {
            return ModelData.ObjectData.Find(objType, setNumber);
        }
        /// <inheritdoc/>

        public ISetInfo GetModelSetInfo(ObjType objType, string setName)
        {
            return ModelData.ObjectData.GetSetInfo(objType, setName);
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllModelObjects()
        {
            return ModelData.ObjectData.GetAllObjects();
        }

        /// <inheritdoc/>

        public IEnumerable<IElement> GetModelElements()
        {
            return ModelData.ObjectData.GetAllElements();
        }
        /// <inheritdoc/>
        public IGroup GetModelGroup(int grIndex)
        {
            return ModelData.GroupData[grIndex];
        }

        /// <inheritdoc/>
        public string MaterialsDB
        {
            get { return project.TaskData.Materials; }
            set { TaskData.Materials = value; }
        }
        /// <inheritdoc/>

        public string FunctionsDB
        {
            get { return project.TaskData.Functions; }
            set { TaskData.Functions = value; }
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
