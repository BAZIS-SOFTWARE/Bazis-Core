using Model.Interfaces;
using Model.Interfaces.ObjectsFinders;
using Model.Interfaces.GeometryObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Sort;
using Model.GeometryObjects;

namespace Model.ObjectsCollections
{
    internal class LineCollection : IObjectsCollection<ILineObject<GeometryPoint>>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Линия} {lines.Count}";
        }
        List<ILineObject<GeometryPoint>> lines = new List<ILineObject<GeometryPoint>>();
        /// <inheritdoc/>

        public int Count => lines.Count;
        /// <inheritdoc/>

        public void Add(ILineObject<GeometryPoint> addObj)
        {
            if (lines.Count > 0)
                if (addObj.Number <= lines.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    lines.Add(addObj);
            else
                lines.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<ILineObject<GeometryPoint>> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<ILineObject<GeometryPoint>>.MergeSortFunction(objsList);

            if (lines.Count > 0)
                if (objsList.First().Number <= lines.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    lines.AddRange(addObjs);
            else
                lines.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            lines.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            lines = lines.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public ILineObject<GeometryPoint> Find(int objNumbr)
        {
            return ByNumberFinder.Find(lines,objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<ILineObject<GeometryPoint>> GetEnumerator()
        {
            foreach (var item in lines)
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