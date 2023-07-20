using System;
using System.Collections.Generic;
using System.Linq;
using Model.GmshWrapper;

namespace Model.GmshObject
{ 
    public class MeshObject : GeometryObject, IMeshApi
    {
        public double Density 
        { 
            get
            {
                SetCurrent();
                return GmshWrapperGeneral.OptionGetNumber("Mesh.MeshSizeFactor");
            }
            set
            {
                SetCurrent();
                GmshWrapperGeneral.OptionSetNumber("Mesh.MeshSizeFactor",value);
            }
        }
        public MeshObject() : base(){}

        public MeshObject(string path) : base(path) { }
        public void Generate(int dim)
        {
            SetCurrent();
            GmshWrapperMesh.Generate(dim);
        }

        public void QuadrangulateMesh()
        {
            SetCurrent();
            GmshWrapperMesh.Recombine();
        }

        public void RefineMesh()
        {
            SetCurrent();
            GmshWrapperMesh.Refine();
        }

        public IEnumerable<Node> GetNodes()
        {
            SetCurrent();
            return GmshWrapperMesh.GetNodes();
        }

        public IEnumerable<ModelObject> GetElement1D()
        {
            SetCurrent();
            return GmshWrapperMesh.GetElements(1, -1);
        }

        /// <summary>
        /// получить плоские элементы
        /// </summary>
        /// <returns>плоские элементы</returns>
        public IEnumerable<ModelObject> GetElement2D()
        {
            SetCurrent();
            return GmshWrapperMesh.GetElements(2, -1);
        }
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        public IEnumerable<ModelObject> GetSurfaces()
        {
            SetCurrent();
            return GmshWrapperMesh.GetElements(2, -1);
        }
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        public IEnumerable<ModelObject> GetSurfaceById(int surfaceId) => GmshWrapperMesh.GetElements(2, surfaceId);
        /// <summary>
        /// получить объёмные элементы
        /// </summary>
        /// <returns>объёмные элементы</returns>
        public IEnumerable<ModelObject> GetElement3D()
        {
            SetCurrent();
            return GmshWrapperMesh.GetElements(3, -1);
        }
        
        public void ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay)
        {
            SetCurrent();
            GmshWrapperGeneral.OptionSetNumber("Mesh.Algorithm3D", Convert.ToDouble(algorithm));
        }

        public void ChangeTriangulationAlgorithm3D(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay)
        {
            SetCurrent();
            GmshWrapperGeneral.OptionSetNumber("Mesh.Algorithm", Convert.ToDouble(algorithm));
        }

        public void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, int curve)
        {
            SetCurrent();
            switch (ta)
            {
                case TransfiniteAlgorithm.Progressive:
                    GmshWrapperMesh.SetTransfiniteCurve(curve, number, "Progression", parameter);
                    break;
                case TransfiniteAlgorithm.Bump:
                    GmshWrapperMesh.SetTransfiniteCurve(curve, number, "Bump", parameter);
                    break;
                case TransfiniteAlgorithm.Beta:
                    GmshWrapperMesh.SetTransfiniteCurve(curve, number, "Beta", parameter);
                    break;
            }
        }
    }
}