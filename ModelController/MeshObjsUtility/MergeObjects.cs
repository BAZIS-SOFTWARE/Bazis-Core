using ModelInterfaces.ObjectsComparers;
using ModelInterfaces;
using System.Collections.Generic;
using System.Linq;

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

        public void Merge<T>(List<List<int>> numbers) where T : IModelObject
        {
            var objs = objectData.FindMany<T>().ToArray();
            var numberObjs = objectData.FindMany<T>().Select(x => x.Number).ToList();

            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                var masterInd = numberObjs.BinarySearch(masterNumber);
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];

                    var slaveInd = numberObjs.BinarySearch(slaveNumber);
                    objs[slaveInd].SetNumber(objs[masterInd].Number);
                }
            }

            var noDublicate = objectData.Distinct(new EqualityComparerByNumber()).ToArray();

            objectData.Clear();
            objectData.AddRange(noDublicate);
        }
    }
}
