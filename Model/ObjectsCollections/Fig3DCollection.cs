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
    public class Fig3DCollection : IObjectsCollection<IFigure3D>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Фигура3D} {figures3D.Count}";
        }

        List<IFigure3D> figures3D = new List<IFigure3D>();
        /// <inheritdoc/>

        public int Count => figures3D.Count;
        /// <inheritdoc/>

        public void Add(IFigure3D addObj)
        {
            if (figures3D.Count > 0)
                if (addObj.Number <= figures3D.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    figures3D.Add(addObj);
            else
                figures3D.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<IFigure3D> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IFigure3D>.MergeSortFunction(objsList);

            if (figures3D.Count > 0)
                if (objsList.First().Number <= figures3D.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    figures3D.AddRange(addObjs);
            else
                figures3D.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            figures3D.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            figures3D = figures3D.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public IFigure3D Find(int objNumbr)
        {
            return figures3D.Find(objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<IFigure3D> GetEnumerator()
        {
            foreach (var item in figures3D)
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