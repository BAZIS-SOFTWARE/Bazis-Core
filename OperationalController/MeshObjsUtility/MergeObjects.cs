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
            var el2DToUpdate = new HashSet<Element2D>();
            var el3DToUpdate = new HashSet<Element3D>();
            //var masterNumbers = modelObjects.Select(x => x.Number).ToList();
            //masterNumbers.Sort();
            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                //найти индкс мастер объекта
                if (!nodes.ContainsKey(masterNumber))
                    continue;
                var masterNode = nodes[masterNumber];
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];
                    if (!nodes.ContainsKey(slaveNumber))
                        continue;
                    var slaveNode = nodes[slaveNumber];

                    // пробуем вариант перестановки ссылки в памяти
                    // вместо замены номера и координат

                    foreach (var elem in slaveNode.GetElements())
                    {
                        if (elem is Element2D el2d)
                            el2DToUpdate.Add(el2d);
                        else if (elem is Element3D el3d)
                            el3DToUpdate.Add(el3d);

                        var index = elem.GetNodeIndex(slaveNode);
                        masterNode.AddElement(elem);
                        elem[index] = masterNode;
                    }

                    nodes.Remove(slaveNumber);
                    //установить ссылку на мастер объект вместо номера мастер объекта
                    //slaveNode.SetNumber(masterNode.Number);// = modelObjects[masterInd].Number;
                    //modelObjects[slaveNode].Position = modelObjects[masterNode].Position;
                }
            }
            foreach (var el2d in el2DToUpdate)
                el2d.UpdateSurface();

            foreach (var el3d in el3DToUpdate)
                el3d.UpdateSurface();
            //return modelObjects.Distinct(new ModelObjectsComparer());
        }
    }
}
