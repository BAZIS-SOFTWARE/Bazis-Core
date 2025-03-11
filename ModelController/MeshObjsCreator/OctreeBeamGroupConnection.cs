using Model.GroupsData;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ModelController.MeshObjsCreator
{
    public class OctreeBeamGroupConnection : BeamGroupConnection
    {
        protected override IEnumerable<Beam> ConnectGroups(Group smaller, Group larger, double searchRadius, int beamsCount, int similarityFactor)
        {
            var octree = CreateOctree(larger.Cast<Node>());
            var incidentEdgesCount = larger.Cast<Node>().ToDictionary(n => n, _ => 0);
            var connectionValue = (beamsCount == 0) ? similarityFactor : beamsCount;
            var startIndex = 0;
            foreach (var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, searchRadius, connectionValue, octree, startIndex, incidentEdgesCount))
                {
                    yield return result;
                    startIndex++;
                }
            }
        }

        private PointOctree<Node> CreateOctree(IEnumerable<Node> allNodes)
        {
            var minX = allNodes.Select(n => n.Position._x).Min();
            var maxX = allNodes.Select(n => n.Position._x).Max();

            var minY = allNodes.Select(n => n.Position._y).Min();
            var maxY = allNodes.Select(n => n.Position._y).Max();

            var minZ = allNodes.Select(n => n.Position._z).Min();
            var maxZ = allNodes.Select(n => n.Position._z).Max();

            var size = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));

            var octree = new PointOctree<Node>(size, new Vector3(maxX - minX, maxY - minY, maxZ - minZ) * 0.5f, 1e-4f);

            foreach (var node in allNodes)
            {
                octree.Add(node, new Vector3(node._x, node._y, node._z));
            }

            return octree;
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, Group group, 
            double searchRadius, int toConnect, PointOctree<Node> octree, int startIndex,
            Dictionary<Node, int> incidentEdgesCount)
        {
            return octree.GetNearby(new Vector3(node._x, node._y, node._z), (float)searchRadius)
                         .OrderBy(n => GetSquaredDistance(n, node))
                         .ThenBy(n => incidentEdgesCount[n])
                         .Take(toConnect)
                         .Select((other, i) =>
                         {
                             incidentEdgesCount[other]++;
                             return new Beam(startIndex + i, new Node[] { node, other });
                         });
        }
    }
}
