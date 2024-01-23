using Functions.Sort;
using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.GeometryObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsCollections
{
    internal class LineCollection : IObjectsCollection<ILineObject<IGeometryPoint>>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Линия} {lines.Count}";
        }
        List<ILineObject<IGeometryPoint>> lines = new List<ILineObject<IGeometryPoint>>();
        /// <inheritdoc/>

        public int Count => lines.Count;
        /// <inheritdoc/>

        public void Add(ILineObject<IGeometryPoint> addObj)
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

        public void AddRange(IEnumerable<ILineObject<IGeometryPoint>> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<ILineObject<IGeometryPoint>>.MergeSortFunction(objsList);

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

        public ILineObject<IGeometryPoint> Find(int objNumbr)
        {
            return lines.Find(objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<ILineObject<IGeometryPoint>> GetEnumerator()
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