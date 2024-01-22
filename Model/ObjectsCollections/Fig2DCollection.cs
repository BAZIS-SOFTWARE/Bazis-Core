using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using System.Collections;
using System.Collections.Generic;

namespace Model.ObjectsCollections
{
    internal class Fig2DCollection : IObjectsCollection<IFigure2D>
    {
        public int Count => throw new System.NotImplementedException();

        public void Add(IFigure2D addObj)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<IFigure2D> addObjs)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void ClearNotExisted()
        {
            throw new System.NotImplementedException();
        }

        public IFigure2D Find(int objNumbr)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IFigure2D> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}