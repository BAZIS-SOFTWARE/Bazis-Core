using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.GeometryObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Sort;

namespace Model.ObjectsCollections
{
    /// <summary>
    /// Набор объемных фигур из поверхностей
    /// </summary>
    public class VolumeFiguresCollection : IObjectsCollection<IVolumeFigure>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Фигура3D} {figures3D.Count}";
        }

        List<IVolumeFigure> figures3D = new List<IVolumeFigure>();
        /// <inheritdoc/>

        public int Count => figures3D.Count;
        /// <inheritdoc/>

        public void Add(IVolumeFigure addObj)
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

        public void AddRange(IEnumerable<IVolumeFigure> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IVolumeFigure>.MergeSortFunction(objsList);

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

        public IVolumeFigure Find(int objNumbr)
        {
            return ByNumberFinder.Find(figures3D,objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<IVolumeFigure> GetEnumerator()
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