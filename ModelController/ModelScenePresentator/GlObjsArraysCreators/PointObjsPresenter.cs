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


        public override Tuple<int, int, int> CreateIndexes()
        {
            var indexes = 0;
            var vertexes = 0;

            foreach (var pointObject in pointObjects)
            {
                vertexes += 3;
                indexes++;
            }


            return new Tuple<int, int, int>(indexes, vertexes, 0);
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

        public override int Count()
        {
            return pointObjects.Length;
        }

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

        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            return null;
        }
    }
}
