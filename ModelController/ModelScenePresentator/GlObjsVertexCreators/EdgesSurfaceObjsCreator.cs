using Geometry;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class EdgesSurfaceObjsCreator : IEdgeObjsVertexCreator
    {
        public class SurfaceEntry
        {
            public List<Surface> surfaces;
            
        }

        private Hashtable surfaces;
        public EdgesSurfaceObjsCreator()
        {
            surfaces = new Hashtable();
        }

        public void Create(ref int stride, bool[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            HashSet<string> surfaces = new HashSet<string>();
            
            if (surfaceElement.ViewState)
            {
                foreach(var surface in surfaceElement.GetSurface())
                {
                    if(surface.ViewState)
                    {
                        


                    }
                    ++i;
                }

            }
        }
    }
}
