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
    public class NodeCollection : IObjectsCollection<INode>
    {
        public int Count => throw new NotImplementedException();

        public void Add(INode addObj)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<INode> addObjs)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ClearNotExisted()
        {
            throw new NotImplementedException();
        }

        public INode Find(int objNumbr)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<INode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
