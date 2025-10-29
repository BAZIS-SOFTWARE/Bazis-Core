using Model.GroupsData;
using Model.Interfaces;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace OperationalController.MeshObjsCreator
{
    public class OctreeBeamGroupConnection : BeamGroupConnection
    {
        private Dictionary<Node, int> incidentEdgesCount;
        private PointOctree<Node> octree;

        protected override IEnumerable<Beam> ConnectGroups(IGroup smaller, IGroup larger)
        {
            octree = CreateOctree(larger.Cast<Node>());
            incidentEdgesCount = larger.Cast<Node>().ToDictionary(n => n, _ => 0);
            var startIndex = 0;
            foreach (var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, startIndex))
                {
                    if (result.CalcLength() < 1e-4f)
                        continue;

                    startIndex = result.Number + 1;
                    yield return result;
                    
                }
            }
        }

        private PointOctree<Node> CreateOctree(IEnumerable<Node> allNodes)
        {
            var minX = allNodes.Min(n => n.Position._x);
            var maxX = allNodes.Max(n => n.Position._x);

            var minY = allNodes.Min(n => n.Position._y);
            var maxY = allNodes.Max(n => n.Position._y);

            var minZ = allNodes.Min(n => n.Position._z);
            var maxZ = allNodes.Max(n => n.Position._z);

            var size = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));

            var octree = new PointOctree<Node>(size, new Vector3(maxX - minX, maxY - minY, maxZ - minZ) * 0.5f, 1e-4f);

            foreach (var node in allNodes)
            {
                octree.Add(node, new Vector3(node._x, node._y, node._z));
            }

            return octree;
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, IGroup group, int startIndex)
        {
            var res = octree.GetNearby(new Vector3(node._x, node._y, node._z), (float)searchRadius)
                         .OrderBy(n => GetSquaredDistance(n, node))
                         //.ThenBy(n => incidentEdgesCount[n]) 
                         // здесь используется для балансировки, если много с одинаковым радиусом
                         // пока не используем
                         .Take(maxIncidentBeams);

            foreach (var item in res)
            {
                yield return new Beam(startIndex, new Node[] { node, item });
                startIndex++;
            }
            // предыдущая форма окончания
                         //.Select((other, i) =>
                         //{
                         //    incidentEdgesCount[other]++;
                         //    return 
                         //});
        }
    }
}
