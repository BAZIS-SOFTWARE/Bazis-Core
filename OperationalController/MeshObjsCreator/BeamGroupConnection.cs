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

        public IEnumerable<Beam> Create(IEnumerable<Node> master, IEnumerable<Node> slave, double radius, int beamsCount = 0)
        {
            if (master.Count() == 0 || slave.Count() == 0)
                throw new CollectionEmptyArgumentException("Одна из коллекций пустая");
            if (radius <= 0)
                throw new NegativeRadiusArgumentException("Радиус поиска должен быть больше нуля");

            searchRadius = radius;
            var k = 1d * master.Count() / slave.Count();
            if(beamsCount == 0)
            {
                if (k <= 1)
                {
                    maxIncidentBeams = (int)Math.Ceiling(1 / k);
                    return Connect(master, slave);
                }
                else
                {
                    maxIncidentBeams = (int)Math.Ceiling(k);
                    return Connect(slave, master);
                }
            }
            maxIncidentBeams = beamsCount;
            if (k <= 1)
                return Connect(master, slave);
            return Connect(slave, master);
        }

        protected double GetSquaredDistance(Node one, Node two)
        {
            var vector = two.Position.Sub(one.Position);
            return vector.GetSquareSumCoords();
        }
        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="smaller"></param>
        /// <param name="larger"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Beam> Connect(IEnumerable<Node> smaller, IEnumerable<Node> larger);
    }
}
