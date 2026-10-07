using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void MergeElements(ObjType objType, string setMaster, string setSlave)
        {
            using (BeginChanges())
            {
                if (setMaster == setSlave)
                    return;
                var data = ModelData.ObjectData;
                switch (objType)
                {
                    case ObjType.Элемент1D:
                        MergeElementSets(data.E1DCollection, setMaster, setSlave);
                        break;
                    case ObjType.Элемент2D:
                        MergeElementSets(data.E2DCollection, setMaster, setSlave);
                        break;
                    case ObjType.Элемент3D:
                        MergeElementSets(data.E3DCollection, setMaster, setSlave);
                        break;
                    default:
                        throw new ArgumentException("Only element sets can be merged.", nameof(objType));
                }
            }
        }

        /// <summary>Объединяет наборы элементов с регистрацией выполненных изменений.</summary>
        private void MergeElementSets<T>(IObjectsCollection<T> collection, string masterName, string slaveName) where T : IModelObject
        {
            var slave = collection[slaveName];
            var master = collection[masterName];
            var hasObjects = slave.Count != 0;
            master.Append(slave);
            if (hasObjects)
                RegisterSetChange(master, ChangeAction.Updated);
            slave.Clear();
            collection.Remove(slaveName);
            RegisterSetChange(slave, ChangeAction.Deleted);
            ClearNotExistedGroupData();
            ModelView.Prune();
        }
    }
}
