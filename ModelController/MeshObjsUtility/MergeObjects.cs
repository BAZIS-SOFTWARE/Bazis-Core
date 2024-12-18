using Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using ModelControllerInterfaces;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// MergeObjects
    /// </summary>
    public class MergeObjects : IObjectsMerger
    {
        /// <inheritdoc/>
        public IEnumerable<T> Merge<T>(List<List<int>>numbers,List<T> modelObjects)
            where T : IModelObject
        {
            var masterNumbers = modelObjects.Select(x => x.Number).ToList();
            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                //найти индкс мастер объекта
                var masterInd = masterNumbers.BinarySearch(masterNumber);
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];

                    var slaveInd = masterNumbers.BinarySearch(slaveNumber);

                    //установить ссылку на мастер объект вместо номера мастер объекта
                    modelObjects[slaveInd] = modelObjects[masterInd];
                }
            }
            return modelObjects.Distinct();
        }
    }
}
