using Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OperationalController;
using Model.MeshObjects;
using Model.Utilities;
using Model.Interfaces.ObjectsCollections;
using Model.Interfaces.MeshObjects;
using System;

namespace OperationalController.MeshObjsUtility
{
    /// <summary>
    /// MergeObjects
    /// </summary>
    public class MergeObjects : IObjectsMerger
    {
        /// <inheritdoc/>
        public void Merge(List<List<int>>numbers,IObjectsSet<Node> nodes)
        {
            Merge(numbers, nodes, out _, out _);
        }

        /// <inheritdoc/>
        public void Merge(List<List<int>> numbers, IObjectsSet<Node> nodes, out IReadOnlyCollection<IElement> changedElements, out bool nodesRemoved)
        {
            var changed = new HashSet<IElement>(ReferenceEqualityComparer.Instance);
            changedElements = changed;
            nodesRemoved = false;
            foreach (var coincidentNumbers in numbers)
            {
                if (coincidentNumbers.Count == 0)
                    continue;
                var masterNumber = coincidentNumbers[0];
                if (!nodes.ContainsKey(masterNumber))
                    continue;
                var masterNode = nodes[masterNumber];
                for (var j = 1; j < coincidentNumbers.Count; j++)
                {
                    var slaveNumber = coincidentNumbers[j];
                    if (slaveNumber == masterNumber || !nodes.ContainsKey(slaveNumber))
                        continue;
                    var slaveNode = nodes[slaveNumber];
                    var elements = slaveNode.GetElements().ToArray();
                    foreach (var elem in elements)
                    {
                        var index = elem.GetNodeIndex(slaveNode);
                        slaveNode.RemoveElement(elem);
                        if (index < 0)
                            continue;
                        var masterElements = masterNode.GetElements();
                        if (!masterElements.Contains(elem))
                            masterNode.AddElement(elem);
                        elem[index] = masterNode;
                        changed.Add(elem);
                    }
                    if (nodes.Remove(slaveNumber))
                    {
                        slaveNode.ExistState = false;
                        nodesRemoved = true;
                    }
                }
            }
        }
    }
}
