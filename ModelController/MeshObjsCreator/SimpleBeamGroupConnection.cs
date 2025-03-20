using Model.GroupsData;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelController.MeshObjsCreator
{
    public class SimpleBeamGroupConnection : BeamGroupConnection
    {
        private Dictionary<Node, int> incidentEdgesCount;

        protected override IEnumerable<Beam> ConnectGroups(Group smaller, Group larger)
        {
            var startIndex = 0;
            incidentEdgesCount = larger.Cast<Node>().ToDictionary(n => n, _ => 0);
            foreach (var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, startIndex))
                {
                    yield return result;
                    startIndex++;
                }
            }
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, Group group, int startIndex)
        {
            var binded = 0;
            foreach (var other in group.Cast<Node>()
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
