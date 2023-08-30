using Geometry;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.ModelScenePresentator.GlObjsPresenters
{
    public abstract class ObjsPresenter
    {
        /// <summary>
        /// GetObjs
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IModelObject> GetObjs();
        /// <summary>
        /// GetObjs
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public abstract IEnumerable<IModelObject> GetObjs(Color color);

        public abstract IModelObject this[int ind] { get; }

        public abstract Tuple<int, int, int,int> CreateIndexes();

        public abstract int[] CreatePointers(int indexLength);

        public abstract float[] CreateVertexes(int vertexLength, string vertexType);

        public abstract bool[] CreateEdgeFlags(int edgesLength);

        public abstract int Count();
        
        public abstract IModelObject FindObj(int objNumbr);
    }
}
