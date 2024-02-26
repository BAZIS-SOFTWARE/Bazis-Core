using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Functions.Sort;

namespace Model.ObjectsCollections
{
    public class Element2DCollection : IObjectsCollection<IElement2D>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Элемент2D} {elements2D.Count}";
        }
        List<IElement2D> elements2D = new List<IElement2D>();
/// <inheritdoc/>

        public int Count => elements2D.Count;
/// <inheritdoc/>

        public void Add(IElement2D addObj)
        {
            if (elements2D.Count > 0)
                if (addObj.Number <= elements2D.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    elements2D.Add(addObj);
            else
                elements2D.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<IElement2D> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IElement2D>.MergeSortFunction(objsList);

            if (elements2D.Count > 0)
                if (objsList.First().Number <= elements2D.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    elements2D.AddRange(addObjs);
            else
                elements2D.AddRange(addObjs);

        }
/// <inheritdoc/>

        public void Clear()
        {
            elements2D.Clear();
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            elements2D = elements2D.Where(x => x.ExistState == true).ToList();
        }
/// <inheritdoc/>

        public IElement2D Find(int objNumbr)
        {
            return elements2D.Find(objNumbr);
        }
/// <inheritdoc/>

        public IEnumerator<IElement2D> GetEnumerator()
        {
            foreach (var item in elements2D)
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
