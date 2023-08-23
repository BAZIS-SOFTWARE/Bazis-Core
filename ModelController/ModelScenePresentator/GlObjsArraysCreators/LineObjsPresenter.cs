using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsArraysCreators
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

        public override Tuple<int, int, int,int, List<int>> CreateIndexes()
        {
            var ptrsLength = 0;
            var coordsLength = 0;
            var colorsLength = 0;
            var edgesLength = 0;
            var ptrs = new List<int>();
            foreach (var lineObj in lineObjects)
            {
                if (lineObj.ViewState)
                {
                    coordsLength = coordsLength + lineObj.NumberOfPoints * 3;
                    ptrsLength = ptrsLength + lineObj.NumberOfPoints;
                    colorsLength = colorsLength + lineObj.NumberOfPoints * 4;
                    edgesLength++;
                    ptrs.Add(ptrsLength);
                }
            }

            return new Tuple<int, int, int,int, List<int>>(ptrsLength, coordsLength, colorsLength, edgesLength, ptrs);
        }



        //public override int[] CreatePointers(int indexLength)
        //{
        //    var stride = 0;
        //    var ptr = new int[indexLength];

        //    foreach (var lineObject in lineObjects)
        //    {
        //        if (lineObject.ViewState)
        //        {
        //            for (int m = 0; m < lineObject.NumberOfPoints; m++)
        //            {
        //                ptr[stride + m] = stride + m;
        //            }
        //            stride = stride + lineObject.NumberOfPoints;
        //        }
        //    }
        //    return ptr;
        //}

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
    }
}
