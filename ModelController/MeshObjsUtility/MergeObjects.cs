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

        public void Merge<T>(List<List<int>> numbers, ObjType objsType)
        {
            var objs = objectData[objsType];
            var numberObjs = objs.Select(x => x.Number).ToList();

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

            var noDublicate = objs.Distinct(new EqualityComparerByNumber()).ToArray();

            objectData[objsType].Clear();
            objectData[objsType].AddRange(noDublicate);
        }
    }
}
