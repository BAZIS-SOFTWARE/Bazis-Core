using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.ObjectsFinders;
using SortLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsCollections
{
    /// <summary>
    /// Набор плоских фигур из поверхностей
    /// </summary>
    public class SurfaceFiguresCollection : IObjectsCollection<ISurfaceFigure>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Фигура2D} {figures2D.Count}";
        }

        List<ISurfaceFigure> figures2D = new List<ISurfaceFigure>();
        /// <inheritdoc/>

        public int Count => figures2D.Count;
        /// <inheritdoc/>

        public void Add(ISurfaceFigure addObj)
        {
            if (figures2D.Count > 0)
                if (addObj.Number <= figures2D.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    figures2D.Add(addObj);
            else
                figures2D.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<ISurfaceFigure> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<ISurfaceFigure>.MergeSortFunction(objsList);

            if (figures2D.Count > 0)
                if (objsList.First().Number <= figures2D.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    figures2D.AddRange(addObjs);
            else
                figures2D.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            figures2D.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            figures2D = figures2D.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public ISurfaceFigure Find(int objNumbr)
        {
            return ByNumberFinder.Find(figures2D,objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<ISurfaceFigure> GetEnumerator()
        {
            foreach (var item in figures2D)
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