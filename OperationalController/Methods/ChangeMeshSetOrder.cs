using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ChangeMeshSetOrder(int elemDim, string name, int order)
        {
            using (BeginChanges())
            {
                switch (elemDim)
                {
                    case 1:
                        ChangeElementOrder(ModelData.ObjectData.E1DCollection[name], order);
                        break;
                    case 2:
                        ChangeElementOrder(ModelData.ObjectData.E2DCollection[name], order);
                        break;
                    case 3:
                        ChangeElementOrder(ModelData.ObjectData.E3DCollection[name], order);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(elemDim), "Element dimension must be from 1 to 3.");
                }
            }
        }

        /// <summary>Изменяет порядок всех элементов и сообщает об изменении набора один раз.</summary>
        private void ChangeElementOrder<T>(IObjectsSet<T> set, int order) where T : IElement
        {
            var changed = false;
            foreach (var element in set.Values)
                if (element.Level != order)
                {
                    element.Level = order;
                    changed = true;
                }
            if (changed)
                RegisterSetChange(set, ChangeAction.Updated);
        }
    }
}
