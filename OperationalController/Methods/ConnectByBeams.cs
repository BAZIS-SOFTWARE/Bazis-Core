using Model;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.MeshObjsCreator;
using OperationalController.ObjsCreator;
using Project;
using Project.Tasks;
using System;
using System.IO;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        OctreeBeamGroupConnection beamGroupConnection { get; } = new OctreeBeamGroupConnection();
        /// <inheritdoc/>

        public void ConnectByBeams(string masterGroup, string slaveGroup, double radius, int maxBeams)
        {
            var masterGr = ModelData.GroupData.Find(masterGroup);

            if (masterGr == null)
                throw new NullReferenceException($"группа {masterGr.Name} не найдена");

            var slaveGr = ModelData.GroupData.Find(slaveGroup);

            if (slaveGr == null)
                throw new NullReferenceException($"группа {slaveGr.Name} не найдена");

            var masterNodes = masterGr.Select(x => ModelData.ObjectData.NodesSet[x.Number]);
            var slaveNodes = slaveGr.Select(x => ModelData.ObjectData.NodesSet[x.Number]);

            var beams = beamGroupConnection.Create(masterNodes, slaveNodes, radius, maxBeams);

            var number = 0;
            if (ModelData.ObjectData.E1DCollection.Count != 0)
                number = ModelData.ObjectData.E1DCollection.GetMaxNumber() + 1;
            
            var name = $"connect_{number}";

            var set = new ObjectsSet<Beam>(name);

            foreach (var item in beams)
            {
                item.SetNumber(number);
                set.Add(item.Number, item);
                number++;
            }

            ModelData.ObjectData.E1DCollection.Add(set);
            PublishSetChange(set, ChangeAction.Created);
        } 
    }
}
