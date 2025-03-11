using Geometry;
using Model.GroupsData;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ModelController.MeshObjsCreator
{
    public abstract class BeamGroupConnection : IBeamGroupConnection
    {
        public IEnumerable<Beam> Create(Group master, Group slave, double searchRadius, int beamsCount = 0)
        {
            if (master.Count == 0 || slave.Count == 0)
                return Enumerable.Empty<Beam>();

            var k = 1d * master.Count / slave.Count;

            return k <= 1
                ? ConnectGroups(master, slave, searchRadius, beamsCount, (int)Math.Ceiling(1 / k))
                : ConnectGroups(slave, master, searchRadius, beamsCount, (int)Math.Ceiling(k));
        }

        protected double GetSquaredDistance(Node one, Node two)
        {
            var vector = two.Position.Sub(one.Position);
            return vector.GetSquareSumCoords();
        }

        protected abstract IEnumerable<Beam> ConnectGroups(Group smaller, Group larger, double searchRadius, int beamsCount, int similarityFactor);
    }
}
