using Model.Interfaces;
using Model.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>Синхронизирует сетку модели с геометрическим контроллером.</summary>
        private void SynchronizeGeometryMesh()
        {
            ClearModelCollection(ObjType.Узел);
            var nodes = GmshController.GetNodes();
            var nodesChanged = AddSetObjects(ModelData.ObjectData.NodesSet, nodes);
            var createdSets = new HashSet<ISetInfo>(ReferenceEqualityComparer.Instance);
            var beams = GmshController.GetMeshSets<Beam>(nodes);
            var beamSets = AddCollectionSets(ModelData.ObjectData.E1DCollection, beams);
            createdSets.UnionWith(beamSets);
            var elements2D = GmshController.GetMeshSets<Element2D>(nodes);
            var surfaceSets = AddCollectionSets(ModelData.ObjectData.E2DCollection, elements2D);
            createdSets.UnionWith(surfaceSets);
            var elements3D = GmshController.GetMeshSets<Element3D>(nodes);
            var volumeSets = AddCollectionSets(ModelData.ObjectData.E3DCollection, elements3D);
            createdSets.UnionWith(volumeSets);
            var surfaceChanges = MarkModelSurfaces(ModelData);
            var updatedSets = new HashSet<ISetInfo>(surfaceChanges, ReferenceEqualityComparer.Instance);
            updatedSets.ExceptWith(createdSets);
            if (nodesChanged)
                updatedSets.Add(ModelData.ObjectData.NodesSet);
            foreach (var set in createdSets)
                RegisterSetChange(set, ChangeAction.Created);
            foreach (var set in updatedSets)
                RegisterSetChange(set, ChangeAction.Updated);
            ModelView.Prune();
        }
    }
}
