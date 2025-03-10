using Model.GroupsData;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ModelController.MeshObjsCreator
{
    public interface IBeamGroupConnection
    {
        IEnumerable<Beam> Create(Group master, Group slave, double searchRadius, int beamCount);
    }

    public class OctreeBeamGroupConnection : IBeamGroupConnection
    {
        public IEnumerable<Beam> Create(Group master, Group slave, double searchRadius, int beamCount = 0)
        {
            if (master.Count == 0 || slave.Count == 0)
                return new List<Beam>();

            var k = 1d * master.Count / slave.Count;

            if (k <= 1)
            {
                var octree = CreateOctree(slave.Cast<Node>());
                var toConnect = (beamCount == 0) ? (int)Math.Ceiling(1 / k) : beamCount;
                return ConnectGroups(master, slave, searchRadius, toConnect, octree);
            }
            else
            {
                var octree = CreateOctree(master.Cast<Node>());
                var toConnect = (beamCount == 0) ? (int)Math.Ceiling(k) : beamCount;
                return ConnectGroups(slave, master, searchRadius, toConnect, octree);
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

        private IEnumerable<Beam> ConnectGroups(Group smaller, Group larger, double searchRadius, int toConnect, PointOctree<Node> octree)
        {
            foreach(var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, (float)searchRadius, toConnect, octree))
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, Group group, float searchRadius, int toConnect, PointOctree<Node> octree)
        { 
            return octree.GetNearby(new Vector3(node._x, node._y, node._z), searchRadius)
                         .OrderBy(n => GetSquaredDistance(n, node))
                         .Take(toConnect)
                         .Select(other => new Beam(1, new Node[] { node, other }));
        }

        private double GetSquaredDistance(Node one, Node two)
        {
            return (two.Position._x - one.Position._x) * (two.Position._x - one.Position._x)
                + (two.Position._y - one.Position._y) * (two.Position._y - one.Position._y)
                + (two.Position._z - one.Position._z) * (two.Position._z - one.Position._z);
        }
    }

    public class SimpleBeamGroupConnection : IBeamGroupConnection
    {
        public IEnumerable<Beam> Create(Group master, Group slave, double searchRadius, int beamCount = 0)
        {
            if (master.Count == 0 || slave.Count == 0)
                return new List<Beam>();

            var k = 1d * master.Count / slave.Count;

            if (k <= 1)
            {
                var toConnect = (beamCount == 0) ? (int)Math.Ceiling(1 / k) : beamCount;
                return ConnectGroups(master, slave, searchRadius, toConnect);
            }
            else
            {
                var toConnect = (beamCount == 0) ? (int)Math.Ceiling(k) : beamCount;
                return ConnectGroups(slave, master, searchRadius, toConnect);
            }
        }

        private IEnumerable<Beam> ConnectGroups(Group smaller, Group larger, double searchRadius, int toConnect)
        {
            foreach (var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, (float)searchRadius, toConnect))
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, Group group, float searchRadius, int toConnect)
        {
            var connected = 0;
            foreach(var other in group.Cast<Node>())
            {
                var squaredDistance = GetSquaredDistance(node, other);
                if(squaredDistance <= searchRadius * searchRadius)
                {
                    yield return new Beam(1, new Node[] { node, other });
                    connected++;
                }
                if (connected >= toConnect)
                    yield break;
            }
        }

        private double GetSquaredDistance(Node one, Node two)
        {
            return (two.Position._x - one.Position._x) * (two.Position._x - one.Position._x)
                + (two.Position._y - one.Position._y) * (two.Position._y - one.Position._y)
                + (two.Position._z - one.Position._z) * (two.Position._z - one.Position._z);
        }
    }
}
