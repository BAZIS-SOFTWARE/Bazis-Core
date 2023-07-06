using Geometry;

namespace Model.GmshUser
{
    public enum Triangulation2DAlgorithm
    {
        MeshAdapt = 1,
        Automatic = 2,
        InitialMeshOnly = 3,
        Delaunay = 5,
        FrontalDelaunay = 6,
        BAMG = 7,
        FrontalDelaunayQuad = 8,
        PackingParallelograms = 9,
        QuasiStructuredQuad = 11
    }

    public enum Triangulation3DAlgorithm
    {
        Delaunay = 1,
        InitialMeshOnly = 3,
        Frontal = 4,
        MMG3D = 7,
        RTree = 9,
        HXT = 10
    }

    public interface IMeshApi
    {
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 2D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        void ChangeTriangulationAlgorithm(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay);
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 3D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        void ChangeTriangulationAlgorithm(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay);
    }

}
