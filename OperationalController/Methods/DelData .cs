using Model;
using Model.GroupsData;
using Model.Interfaces;
using Model.MeshObjects;
using Model.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ClearGroupData()
        {
            ModelData.GroupData?.Clear();
            TaskData.Clear();
        }
        /// <summary>
        /// ClearNotExistedTaskData
        /// </summary>
        /// <param name="group"></param>
        public void DeleteCond(IGroup group)
        {
            TaskData.ClearNotExisted(group);
        }
        /// <inheritdoc/>
        public void ClearNotExistedCondData()
        {
                TaskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>
        public void ClearNotExistedModelData()
        {
            ModelData.ObjectData?.ClearNotExisted();
            ModelData.ObjectData?.ClearEmptySet();
            ModelData.GroupData?.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);

            //project.ClearEmptySet();
            //project.ClearNotExistedGroupData();
            //project.ClearNotExistedCondData();

        }
        /// <inheritdoc/>
        public void ClearNotExistedGroupData()
        {
            ModelData.GroupData?.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);
        }
/// <inheritdoc/>

        public void ClearEmptySet()
        {
            ModelData.ObjectData?.ClearEmptySet();
        }
        /// <inheritdoc/>

        public void ClearTaskData()
        {
            TaskData.Clear();
        }
        /// <inheritdoc/>

        public void ClearAllData()
        {
            TaskData.Clear();
            ModelData.ClearAll();
        }

        /// <inheritdoc/>

        public void ClearModelCollection(ObjType objType)
        {
            ModelData.ObjectData.Clear(objType);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>

        public void DeleteModelGroup(string grName)
        {
            var gr = ModelData.GroupData.Find(grName);
            if (gr != null)
            {
                ModelData.GroupData.Remove(gr);
                TaskData.ClearNotExisted(gr);
            }
        }

        /// <inheritdoc/>
        public bool DeleteCond(int index)
        {
            var data = TaskData[index];
            return TaskData.Remove(data);
        }
        /// <inheritdoc/>

        public void DeleteModelSet(ObjType objType, string name)
        {
            ModelData.ObjectData.Remove(objType, name);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>

        public void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs)
        {
            // TODO если геометрия, то удалить по типу и номеру объекта

            foreach (var numb in objNumbs)
            {
                var obj = ModelData.ObjectData.Find(objType, numb);
                if (obj != null)
                    obj.ExistState = false;
            }
 
            ClearNotExistedModelData();
        }

        public void RemoveMultiPassSection()
        {
            var oldItemsToRemove = ModelData.ObjectData.E2DCollection.Objects.Where(item => item.Name.Contains("MultiPassSection")).ToList();
            if (oldItemsToRemove.Count != 0)
            {
                var nodes = new HashSet<Node>();
                foreach (var item in oldItemsToRemove)
                {
                    GetNodes(item);
                    ModelData.ObjectData.E2DCollection.Remove(item.Name);
                }

                var extrudeSet = ModelData.ObjectData.E3DCollection.Objects.Where(item => item.Name.Contains("extrude")).ToList();
                foreach (var extruder in extrudeSet)
                {
                    GetNodes(extruder);
                    ModelData.ObjectData.E3DCollection.Remove(extruder.Name);
                }

                foreach (var node in nodes)
                    ModelData.ObjectData.NodesSet.Remove(node.Number);
                ModelData.ObjectData.NodesSet.ClearNotExisted();

                void GetNodes<TElement>(ObjectsSet<TElement> elments) where TElement : Element
                {
                    foreach (var value in elments.Values)
                        foreach (var node in value)
                            nodes.Add(node);
                }
            }
        }
    }
}
