using Model.Interfaces;
using Model.MeshObjects;
using OperationalController.MeshObjsUtility;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void CreateGroup(IEnumerable<IModelObject> objs)
        {
            ModelData.GroupData.Create(objs);
        }
        /// <inheritdoc/>
        public void CreateGroup(string grName, IEnumerable<IModelObject> objs)
        {

            ModelData.GroupData.Create(grName, objs);
        }

        /// <inheritdoc/>
        public string CreateElementsGroup(ObjType objType, string setName)
        {
            var objs = ModelData.ObjectData.GetObjects(objType, setName);
            return ModelData.GroupData.Create(objs);
        }

        /// <inheritdoc/>
        public string BoundaryNodesGroup(string setName)
        {
            var finder = new FindOpenSurfaceNodes();
            var surfaceNodes = finder.Find(ModelData.ObjectData.E3DCollection.GetObjects());

            var ndsList = surfaceNodes.Select(x => ModelData.ObjectData.NodesSet[x]);

            return ModelData.GroupData.Create(ndsList);
        }
    }
}
