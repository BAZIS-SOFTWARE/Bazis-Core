using Model;
using Model.ObjectsComparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// MergeObjects
    /// </summary>
    public class MergeObjects
    {
        ObjectsData objectData;
        /// <summary>
        /// MergeObjects
        /// </summary>
        /// <param name="objectData"></param>
        public MergeObjects(ObjectsData objectData)
        {
            this.objectData = objectData;
        }

        public void Merge<T>(List<List<int>> numbers) where T : ModelObject
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
