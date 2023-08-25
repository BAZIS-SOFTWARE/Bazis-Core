using Model;
using Model.Interfaces;
using ModelController.MeshObjsUtility;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;

namespace ModelController.ModelScenePresentator.GlObjsArraysCreators
{
    public class PointObjsPresenter : ObjsPresenter
    {

        IModelObject[] pointObjects;
        public PointObjsPresenter(IModelObject[] pointObjects)
        {
            this.pointObjects = pointObjects;
        }
/// <inheritdoc/>


        public override Tuple<int, int, int,int> CreateIndexes()
        {
            var indexes = 0;
            var coords = 0;
            var colors = 0;

            foreach (var pointObject in pointObjects)
            {
                //Закоментировано т.к вызывает exception
                //if (pointObject.ViewState)
                //{
                    coords += 3;
                    indexes++;
                    colors += 4;
                //}
            }


            return new Tuple<int, int, int,int>(indexes, coords, colors,0);
        }


        public override int[] CreatePointers(int indexLength)
        {
            var ptr = new int[indexLength];
            for (int i = 0; i < pointObjects.Length; i++)
            {
                if (pointObjects[i].ViewState)
                {
                    ptr[i] = i;
                }
            }
            return ptr;
        }
        /// <inheritdoc/>

        public override float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;

            IPointObjsVertexCreator vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorPointObjsCreator();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsPointObjsCreator();
            else vertexCreator = new NormalsPointObjsCreator();

            for (int i = 0; i < pointObjects.Length; i++)
            {
                vertexCreator.Create(ref stride, vertexArray, pointObjects[i]);
            }

            return vertexArray;
        }
/// <inheritdoc/>

        public override int Count()
        {
            return pointObjects.Length;
        }
/// <inheritdoc/>

        public override IEnumerable<IModelObject> GetObjs()
        {
            foreach (var pointObject in pointObjects)
            {
                yield return pointObject;
            }
        }

        public override IModelObject this[int index]
        {
            get{return pointObjects[index];}
        }

        public IEnumerable<int> FindSurfacePoints(Element3D [] elements)
        {

            var normalCalculator = new NormalsCalculator(elements);
            normalCalculator.CalcNodeNormals();


            var surfacedNodesFinder = new FindOpenSurfaceNodes();
            return surfacedNodesFinder.Find(elements);
        }
/// <inheritdoc/>

        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            return new bool[0];
        }
    }
}
