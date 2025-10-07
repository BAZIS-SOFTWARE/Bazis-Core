using Geometry;
using Model.GroupsData;
using Model.Interfaces;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OperationalController.MeshObjsCreator
{
    public abstract class BeamGroupConnection
    {
        protected double searchRadius;
        protected int maxIncidentBeams;

        public IEnumerable<Beam> Create(IGroup master, IGroup slave, double radius, int beamsCount = 0)
        {
            if (master.Count == 0 || slave.Count == 0)
                throw new Exception("Одна из групп пустая");
            if (radius <= 0)
                throw new Exception("Радиус поиска должен быть больше нуля");

            searchRadius = radius;
            var k = 1d * master.Count / slave.Count;
            if(beamsCount == 0)
            {
                if (k <= 1)
                {
                    maxIncidentBeams = (int)Math.Ceiling(1 / k);
                    return ConnectGroups(master, slave);
                }
                else
                {
                    maxIncidentBeams = (int)Math.Ceiling(k);
                    return ConnectGroups(slave, master);
                }
            }
            maxIncidentBeams = beamsCount;
            if (k <= 1)
                return ConnectGroups(master, slave);
            return ConnectGroups(slave, master);
        }

        protected double GetSquaredDistance(Node one, Node two)
        {
            var vector = two.Position.Sub(one.Position);
            return vector.GetSquareSumCoords();
        }

        protected abstract IEnumerable<Beam> ConnectGroups(IGroup smaller, IGroup larger);
    }
}
