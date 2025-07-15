using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using OperationalController.Extensions;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using Newtonsoft.Json.Linq;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public IEnumerable<ISetInfo> GetModelSetsInfo(string objects)
        {
            var objType = objects.ToEnum<ObjType>();
            foreach (var item in ModelData.ObjectData.GetSetsInfo(objType))
                yield return item;

        }
        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetModelObjects(string objects)
        {
            var objType = objects.ToEnum<ObjType>();
            return ModelData.ObjectData.GetObjects(objType);
        }
        /// <inheritdoc/>

        public ISetInfo GetModelSetInfo(string objects, string setName)
        {
            var objType = objects.ToEnum<ObjType>();
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
            get { return GeneralData.Materials; }
            set { GeneralData.Materials = value; }
        }
        /// <inheritdoc/>

        public string FunctionsDB
        {
            get { return GeneralData.Functions; }
            set { GeneralData.Functions = value; }
        }

        /// <inheritdoc/>
        public TaskKind ProjectKind
        {
            get 
            {
                return GeneralData.TaskKind;
            }
            set
            {
                TaskData.Clear();
                GeneralData.TaskKind = value;
            }         
        }
        /// <inheritdoc/>
        public TaskType GetProjectType
        {
            get
            {
                return GeneralData.TaskType;
            }
            set
            {
                TaskData.Clear();
                GeneralData.TaskType = value;
            }
        }
    }
}
