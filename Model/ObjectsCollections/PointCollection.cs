using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using System.Collections;
using System.Collections.Generic;

namespace Model.ObjectsCollections
{
    internal class PointCollection : IObjectsCollection<IGeometryPoint>
    {
        public int Count => throw new System.NotImplementedException();

        public void Add(IGeometryPoint addObj)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<IGeometryPoint> addObjs)
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

        public IGeometryPoint Find(int objNumbr)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IGeometryPoint> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}