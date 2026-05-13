using Model.GroupsData;
using Model.Interfaces;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationalController.MeshObjsCreator
{
    public class SimpleBeamGroupConnection : BeamGroupConnection
    {
        private Dictionary<Node, int> incidentEdgesCount;

        protected override IEnumerable<Beam> Connect(IEnumerable<Node> smaller, IEnumerable<Node> larger)
        {
            var startIndex = 0;
            incidentEdgesCount = larger.ToDictionary(n => n, _ => 0);
            foreach (var node in smaller)
            {
                foreach (var result in ConnectOneToMany(node, larger, startIndex))
                {
                    yield return result;
                    startIndex++;
                }
            }
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, IEnumerable<Node> slave, int startIndex)
        {
            var binded = 0;
            foreach (var other in slave
                                       .Where(n => GetSquaredDistance(node, n) <= searchRadius * searchRadius)
                                       .OrderBy(n => GetSquaredDistance(node, n))
                                       .ThenBy(n => incidentEdgesCount[n])
                                       .Take(maxIncidentBeams))
            {
                incidentEdgesCount[other]++;
                yield return new Beam(startIndex + binded, new Node[] { node, other });
                binded++;
            }
        }
    }
}
