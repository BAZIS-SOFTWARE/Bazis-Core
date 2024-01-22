using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using System.Collections;
using System.Collections.Generic;

namespace Model.ObjectsCollections
{
    internal class LineCollection : IObjectsCollection<ILineObject<IGeometryPoint>>
    {
        public int Count => throw new System.NotImplementedException();

        public void Add(ILineObject<IGeometryPoint> addObj)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<ILineObject<IGeometryPoint>> addObjs)
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

        public ILineObject<IGeometryPoint> Find(int objNumbr)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<ILineObject<IGeometryPoint>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}