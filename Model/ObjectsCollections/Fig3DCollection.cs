using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using System.Collections;
using System.Collections.Generic;

namespace Model.ObjectsCollections
{
    internal class Fig3DCollection : IObjectsCollection<IFigure3D>
    {
        public int Count => throw new System.NotImplementedException();

        public void Add(IFigure3D addObj)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<IFigure3D> addObjs)
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

        public IFigure3D Find(int objNumbr)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IFigure3D> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}