using Functions.Sort;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.ObjectsFinders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.ObjectsCollections
{
    public class Fig2DCollection : IObjectsCollection<IFigure2D>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Фигура2D} {figures2D.Count}";
        }

        List<IFigure2D> figures2D = new List<IFigure2D>();
        /// <inheritdoc/>

        public int Count => figures2D.Count;
        /// <inheritdoc/>

        public void Add(IFigure2D addObj)
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

        public void AddRange(IEnumerable<IFigure2D> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IFigure2D>.MergeSortFunction(objsList);

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

        public IFigure2D Find(int objNumbr)
        {
            return figures2D.Find(objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<IFigure2D> GetEnumerator()
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