using Model.Interfaces;
using Model.ObjectsFinders;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using ModelControllerInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsPresenters
{
    public class LineObjsPresenter : IObjsPresenter
    {
        ILineObject [] lineObjects;

        public IModelObject this[int index]
        {
            get { return lineObjects[index]; }
        }

        public LineObjsPresenter(ILineObject[] lineObjects)
        {
            this.lineObjects = lineObjects;
        }

        public Tuple<int, int, int,int> CreateIndexes()
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



        public int[] CreatePointers(int indexLength)
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

        public float[] CreateVertexes(int vertexLength, string vertexType)
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

        public int Count()
        {
            return lineObjects.Length;
        } 

        public IEnumerable<IModelObject> GetObjs()
        {
            foreach (var lineObject in lineObjects)
            {
                yield return lineObject;
            }
        }

        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edgesArray = new bool[edgesLength];

            edgesArray.Where(x => x = true);

            return edgesArray;
        }

        public IModelObject FindObj(int objNumbr)
        {
            return ByNumberFinder.Find(lineObjects, objNumbr);
        }
/// <inheritdoc/>

        public IEnumerable<IModelObject> GetObjs(Color color)
        {
            foreach (var lineObject in lineObjects)
            {
                if (lineObject.MasterColor == color)
                    yield return lineObject;
            }
        }
    }
}
