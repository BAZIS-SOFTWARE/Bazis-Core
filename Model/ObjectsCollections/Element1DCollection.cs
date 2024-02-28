using Functions.Sort;
using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsCollections
{
    public class Element1DCollection : IObjectsCollection<ILineObject<INode>>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Элемент1D} {elements1D.Count}";
        }

        List<ILineObject<INode>> elements1D = new List<ILineObject<INode>>();
        /// <inheritdoc/>

        public int Count => elements1D.Count;
        /// <inheritdoc/>

        public void Add(ILineObject<INode> addObj)
        {
            if (elements1D.Count > 0)
                if (addObj.Number <= elements1D.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    elements1D.Add(addObj);
            else
                elements1D.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<ILineObject<INode>> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<ILineObject<INode>>.MergeSortFunction(objsList);

            if (elements1D.Count > 0)
                if (objsList.First().Number <= elements1D.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    elements1D.AddRange(addObjs);
            else
                elements1D.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            elements1D.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            elements1D = elements1D.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public ILineObject<INode> Find(int objNumbr)
        {
            return elements1D.Find(objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<ILineObject<INode>> GetEnumerator()
        {
            foreach (var item in elements1D)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
