using Model.Interfaces;
using Model.Interfaces.ObjectsFinders;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Sort;
using Model.MeshObjects;

namespace Model.ObjectsCollections
{
    public class NodeCollection : IObjectsCollection<Node>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Узел} {nodes.Count}";
        }
        List<Node> nodes = new List<Node>();
        /// <inheritdoc/>

        public int Count => nodes.Count;
        /// <inheritdoc/>

        public void Add(Node addObj)
        {
            if (nodes.Count > 0)
                if (addObj.Number <= nodes.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    nodes.Add(addObj);
            else
                nodes.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<Node> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<Node>.MergeSortFunction(objsList);

            if (nodes.Count > 0)
                if (objsList.First().Number <= nodes.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    nodes.AddRange(addObjs);
            else
                nodes.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            nodes.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            nodes = nodes.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public Node Find(int objNumbr)
        {
            return ByNumberFinder.Find(nodes,objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<Node> GetEnumerator()
        {
            foreach (var item in nodes)
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
