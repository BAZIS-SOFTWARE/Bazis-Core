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
                throw new Exception($"группа {masterGr.Name} не найдена");

            var slaveGr = ModelData.GroupData.Find(slaveGroup);

            if (slaveGr == null)
                throw new Exception($"группа {slaveGr.Name} не найдена");

            var beams = beamGroupConnection.Create(masterGr, slaveGr, radius, maxBeams);

            var number = 0;
            if (ModelData.ObjectData.E1DCollection.Count != 0)
                number = ModelData.ObjectData.E1DCollection.GetMaxNumber() + 1;
            
            var name = $"connect_{number}";

            ModelData.ObjectData.E1DCollection.AddRange(name, beams);
        }
    }
}
