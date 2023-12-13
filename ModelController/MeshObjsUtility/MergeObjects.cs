using ModelInterfaces.ObjectsComparers;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System.Collections.Generic;
using System.Linq;
using Functions.Sort;
using Model;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// MergeObjects
    /// </summary>
    public class MergeObjects
    {
        IObjectsData objectData;
        /// <summary>
        /// MergeObjects
        /// </summary>
        /// <param name="objectData"></param>
        public MergeObjects(IObjectsData objectData)
        {
            this.objectData = objectData;
        }
        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="numbers"></param>
        public List<T> Merge<T>(List<List<int>> numbers) where T : ModelObject
        {
            var masterObjs = objectData.FindMany<T>().ToList();
            var masterNumbers = masterObjs.Select(x => x.Number).ToList();
            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];

                    var slaveInd = masterNumbers.BinarySearch(slaveNumber);
                    masterObjs[slaveInd].SetNumber(masterNumber);
                }
            }

            var noDublicate = masterObjs.Distinct(new EqualityComparerByNumber<T>()).ToList();
            MergeSort<T>.MergeSortFunction(noDublicate);

            return noDublicate;
        }
    }
}
