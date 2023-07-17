using System;
using Model.GmshWrapper;

namespace Model.GmshObject
{
    /// <summary>
    /// Алгоритмы триангуляции при построении сетки    
    /// </summary>
    public enum Triangulation2DAlgorithm
    {
        /// <summary>MeshAdapt</summary>
        MeshAdapt = 1,
        /// <summary>Automatic</summary>
        Automatic = 2,
        /// <summary>InitialMeshOnly</summary>
        InitialMeshOnly = 3,
        /// <summary>Delaunay</summary>
        Delaunay = 5,
        /// <summary>FrontalDelaunay</summary>
        FrontalDelaunay = 6,
        /// <summary>BAMG</summary>
        BAMG = 7,
        /// <summary>FrontalDelaunayQuad</summary>
        FrontalDelaunayQuad = 8,
        /// <summary>PackingParallelograms</summary>
        PackingParallelograms = 9,
        /// <summary>QuasiStructuredQuad</summary>
        QuasiStructuredQuad = 11
    }
    /// <summary>
    /// Алгоритмы триангуляции при построении объемов
    /// </summary>
    public enum Triangulation3DAlgorithm
    {
        /// <summary>Delaunay</summary>
        Delaunay = 1,
        /// <summary>InitialMeshOnly</summary>
        InitialMeshOnly = 3,
        /// <summary>Frontal</summary>
        Frontal = 4,
        /// <summary>MMG3D</summary>
        MMG3D = 7,
        /// <summary>RTree</summary>
        RTree = 9,
        /// <summary>HXT</summary>
        HXT = 10
    }
    public class Mesh : Model, IMeshApi
    {
        public void ChangeTriangulationAlgorithm(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay)
        {
            throw new NotImplementedException();
        }

        public void ChangeTriangulationAlgorithm(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay)
        {
            throw new NotImplementedException();
        }

        public void SetMeshPointsOnCurve(int number, GmshUser.TransfiniteAlgorithm ta, double parameter, Curve[] curve)
        {
            throw new NotImplementedException();
        }

        public void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, Curve[] curve)
        {
            GmshWrapperModel.ModelSetCurrent(Id.ToString());
            for (var i = 0; i < curve.Length; ++i)
            {
                switch (ta)
                {
                    case TransfiniteAlgorithm.Progressive:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Progression", parameter);
                        break;
                    case TransfiniteAlgorithm.Bump:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Bump", parameter);
                        break;
                    case TransfiniteAlgorithm.Beta:
                        GmshWrapperMesh.SetTransfiniteCurve(curve[i].Number, number, "Beta", parameter);
                        break;
                }
            }
        }
    }
}
