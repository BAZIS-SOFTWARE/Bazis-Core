using ModelInterfaces.ObjectsComparers;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System.Collections.Generic;
using System.Linq;
using Functions.Sort;
using Model;
using Model.ObjectsCollections;
using ModelInterfaces.MeshObjects;

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
        /// <param name="objType"></param>
        public void Merge(List<List<int>> numbers,ObjType objType)
        {
            var masterObjs = objectData.GetObjects(objType).ToList();
            var masterNumbers = masterObjs.Select(x => x.Number).ToList();
            for (int i = 0; i < numbers.Count; i++)
            {
                var masterNumber = numbers[i][0];
                //найти индкс мастер объекта
                var masterInd = masterNumbers.BinarySearch(masterNumber);
                for (int j = 1; j < numbers[i].Count; j++)
                {
                    var slaveNumber = numbers[i][j];

                    var slaveInd = masterNumbers.BinarySearch(slaveNumber);

                    //установить ссылкуна мастер объект вместо номера мастер объекта
                    masterObjs[slaveInd] = masterObjs[masterInd];
                }
            }
            //var noDublicate = masterObjs.Distinct(new EqualityComparerByNumber<IModelObject>()).ToList();
            //MergeSort<IModelObject>.MergeSortFunction(noDublicate);
            //NodeCollection.Clear();
            //NodeCollection.AddRange(noDublicate);
        }
    }
}
