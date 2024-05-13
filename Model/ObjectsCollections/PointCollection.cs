using SortLib;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.ObjectsFinders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsCollections
{
    public class PointCollection : IObjectsCollection<IGeometryPoint>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Точка} {points.Count}";
        }
        List<IGeometryPoint> points = new List<IGeometryPoint>();
        /// <inheritdoc/>

        public int Count => points.Count;
        /// <inheritdoc/>

        public void Add(IGeometryPoint addObj)
        {
            if (points.Count > 0)
                if (addObj.Number <= points.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    points.Add(addObj);
            else
                points.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<IGeometryPoint> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IGeometryPoint>.MergeSortFunction(objsList);

            if (points.Count > 0)
                if (objsList.First().Number <= points.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    points.AddRange(addObjs);
            else
                points.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            points.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            points = points.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public IGeometryPoint Find(int objNumbr)
        {
            return ByNumberFinder.Find(points,objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<IGeometryPoint> GetEnumerator()
        {
            foreach (var item in points)
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