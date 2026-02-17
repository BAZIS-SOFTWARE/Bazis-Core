using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model;
using Model.MeshObjects;
using OperationalController.MeshObjsCreator;

namespace OperationalController
{
    public partial class ProjectController
    {
        public List<List<Tetra>> ExtrudeAlongVector(IEnumerable<Element2D> layer, Point3D dir, int layers = 1, float layerSize = 1)
        {
            var nodeStart = 0;
            var tetraStart = 0;

            if (ModelData != null)
            {
                if (ModelData.ObjectData.NodesSet != null)
                    nodeStart = ModelData.ObjectData.NodesSet.Max(v => v.Value.Number) + 1;
                if (ModelData.ObjectData.E3DCollection != null)
                    tetraStart = ModelData.ObjectData.E3DCollection.Max(v => v.Value.Number) + 1;
            }
            else
                nodeStart = layer.SelectMany(v => v.GetVertexes()).Max(v => v.Number) + 1;

            var extruder = new TetraExtruder(nodeStart, tetraStart);
            if (ModelData != null)
                foreach (var node in ModelData.ObjectData.NodesSet)
                    if (!extruder.Nodes.ContainsKey(node.Value.Number))
                        extruder.Nodes.Add(node.Value.Number, node.Value);

            return extruder.ExtrudeAlongVector(layer, dir, layers, layerSize);
        }
    }
}
