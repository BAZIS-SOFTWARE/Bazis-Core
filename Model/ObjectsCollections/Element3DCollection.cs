using Functions.Sort;
using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ObjectsCollections
{
    public class Element3DCollection : IObjectsCollection<IElement3D>
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $@"{ObjType.Элемент3D} {elements3D.Count}";
        }
        List<IElement3D> elements3D = new List<IElement3D>();
        /// <inheritdoc/>

        public int Count => elements3D.Count;
        /// <inheritdoc/>

        public void Add(IElement3D addObj)
        {
            if (elements3D.Count > 0)
                if (addObj.Number <= elements3D.Last().Number)
                    throw new Exception($"Последний элемент в коллекции больше добавляемого!");
                else
                    elements3D.Add(addObj);
            else
                elements3D.Add(addObj);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<IElement3D> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IElement3D>.MergeSortFunction(objsList);

            if (elements3D.Count > 0)
                if (objsList.First().Number <= elements3D.Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции больше существующего последнего!");
                else
                    elements3D.AddRange(addObjs);
            else
                elements3D.AddRange(addObjs);

        }
        /// <inheritdoc/>

        public void Clear()
        {
            elements3D.Clear();
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            elements3D = elements3D.Where(x => x.ExistState == true).ToList();
        }
        /// <inheritdoc/>

        public IElement3D Find(int objNumbr)
        {
            return elements3D.Find(objNumbr);
        }
        /// <inheritdoc/>

        public IEnumerator<IElement3D> GetEnumerator()
        {
            foreach (var item in elements3D)
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
