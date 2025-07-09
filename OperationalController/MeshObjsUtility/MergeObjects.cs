using Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OperationalController;
using Model.MeshObjects;
using Model.Utilities;
using Model.Interfaces.ObjectsCollections;

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
            //var masterNumbers = modelObjects.Select(x => x.Number).ToList();
            //masterNumbers.Sort();
            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                //найти индкс мастер объекта
                var masterNode = nodes[masterNumber];
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];

                    var slaveNode = nodes[slaveNumber];
                    slaveNode.SetNumber(masterNode.Number);
                    slaveNode.Position = masterNode.Position;

                    nodes.Remove(slaveNumber);
                    //установить ссылку на мастер объект вместо номера мастер объекта
                    //slaveNode.SetNumber(masterNode.Number);// = modelObjects[masterInd].Number;
                    //modelObjects[slaveNode].Position = modelObjects[masterNode].Position;
                }
            }
            //return modelObjects.Distinct(new ModelObjectsComparer());
        }
    }
}
