using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.ModelScenePresentator.GlObjsArraysCreators
{
    public abstract class ObjsPresenter
    {
        public abstract IEnumerable<IModelObject> GetObjs();

        public abstract IModelObject this[int ind] { get; }

        public abstract Tuple<int, int, int,int> CreateIndexes();

        public abstract int[] CreatePointers(int indexLength);

        public abstract float[] CreateVertexes(int vertexLength, string vertexType);

        public abstract bool[] CreateEdgeFlags(int edgesLength);

        public abstract int Count();

    }
}
