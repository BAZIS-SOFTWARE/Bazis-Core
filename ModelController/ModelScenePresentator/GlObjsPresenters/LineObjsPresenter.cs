using Model;
using Model.Interfaces;
using Model.Utilities;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsPresenters
{
    public class LineObjsPresenter : ObjsPresenter
    {
        ILineObject [] lineObjects;

        public override IModelObject this[int index]
        {
            get { return lineObjects[index]; }
        }

        public LineObjsPresenter(ILineObject[] lineObjects)
        {
            this.lineObjects = lineObjects;
        }

        public override Tuple<int, int, int,int> CreateIndexes()
        {
            var indexes = 0;
            var coords = 0;
            var colors = 0;
            var edges = 0;
            foreach (var lineObj in lineObjects)
            {
                if (lineObj.ViewState)
                {
                    coords = coords + lineObj.NumberOfPoints * 3;
                    indexes = indexes + lineObj.NumberOfPoints;
                    colors = colors + lineObj.NumberOfPoints * 4;
                    edges++;
                }
            }

            return new Tuple<int, int, int,int>(indexes, coords, colors, edges);
        }



        public override int[] CreatePointers(int indexLength)
        {
            var stride = 0;
            var ptr = new int[indexLength];

            foreach (var lineObject in lineObjects)
            {
                if (lineObject.ViewState)
                {
                    for (int m = 0; m < lineObject.NumberOfPoints; m++)
                    {
                        ptr[stride + m] = stride + m;
                    }
                    stride = stride + lineObject.NumberOfPoints;
                }
            }
            return ptr;
        }

        public override float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;

            ILineObjsVertexCreator vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorLineObjsCreator();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsLineObjsCreator();
            else vertexCreator = new NormalsLineObjsCreator();

            foreach (var lineObject in lineObjects)
            {
                if(lineObject.ViewState)
                    vertexCreator.Create(ref stride, vertexArray, lineObject);
            }

            return vertexArray;
        }

        public override int Count()
        {
            return lineObjects.Length;
        } 

        public override IEnumerable<IModelObject> GetObjs()
        {
            foreach (var lineObject in lineObjects)
            {
                yield return lineObject;
            }
        }

        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            var edgesArray = new bool[edgesLength];

            edgesArray.Where(x => x = true);

            return edgesArray;
        }

        public override IModelObject FindObj(int objNumbr)
        {
            return ObjectsFinder.Find(lineObjects, objNumbr);
        }
    }
}
