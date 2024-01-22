using Model.MeshObjects;
using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ObjectsCollections
{
    public class Element1DCollection : IObjectsCollection<ILineObject<INode>>
    {

        /// <inheritdoc/>
        public int Count => throw new NotImplementedException();
/// <inheritdoc/>

        public void Add(ILineObject<INode> addObj)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<ILineObject<INode>> addObjs)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void Clear()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            throw new NotImplementedException();
        }

        public ILineObject<INode> Find(int objNumbr)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ILineObject<INode>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
