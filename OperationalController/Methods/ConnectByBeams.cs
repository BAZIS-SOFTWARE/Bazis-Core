using Model.IO;
using Model.IO.STL;
using OperationalController.MeshObjsCreator;
using OperationalController.ObjsCreator;
using Project;
using Project.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        OctreeBeamGroupConnection beamGroupConnection { get; } = new OctreeBeamGroupConnection();
        /// <inheritdoc/>

        public void ConnectByBeams(string masterGroup, string slaveGroup, double radius, int maxBeams)
        {
            var masterGr = ModelData.GroupData.Find(masterGroup);
            var slaveGr = ModelData.GroupData.Find(slaveGroup);

            var beams = beamGroupConnection.Create(masterGr, slaveGr, radius, maxBeams);

            ModelData.ObjectData.E1DCollection.AddRange("",beams);
        }
    }
}
