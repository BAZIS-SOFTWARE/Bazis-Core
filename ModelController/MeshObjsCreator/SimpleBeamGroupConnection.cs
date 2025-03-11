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
        protected override IEnumerable<Beam> ConnectGroups(Group smaller, Group larger, double searchRadius, int beamsCount, int similarityFactor)
        {
            var startIndex = 0;
            var incidentEdgesCount = larger.Cast<Node>().ToDictionary(n => n, _ => 0);
            var connectionValue = (beamsCount == 0) ? similarityFactor : beamsCount;
            foreach (var node in smaller.Cast<Node>())
            {
                foreach (var result in ConnectOneToMany(node, larger, searchRadius, connectionValue, startIndex, incidentEdgesCount))
                {
                    yield return result;
                    startIndex++;
                }
            }
        }

        private IEnumerable<Beam> ConnectOneToMany(Node node, Group group, double searchRadius, int maxBinded, int startIndex, Dictionary<Node, int> incidentEdgesCount)
        {
            var binded = 0;
            foreach (var other in group.Cast<Node>().OrderBy(n => incidentEdgesCount[n]))
            {
                var squaredDistance = GetSquaredDistance(node, other);
                if (squaredDistance <= searchRadius * searchRadius)
                {
                    incidentEdgesCount[other]++;
                    yield return new Beam(startIndex + binded, new Node[] { node, other });
                    binded++;
                }
                if (binded >= maxBinded)
                    yield break;
            }
        }
    }
}
