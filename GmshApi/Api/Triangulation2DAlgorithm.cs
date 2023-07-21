namespace GmshApi
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
        //InitialMeshOnly = 3, Не работает в Gmsh
        /// <summary>Delaunay</summary>
        Delaunay = 5,
        /// <summary>FrontalDelaunay</summary>
        FrontalDelaunay = 6,
        //BAMG = 7, Не работает в Gmsh
        /// <summary>FrontalDelaunayQuad</summary>
        FrontalDelaunayQuad = 8,
        /// <summary>PackingParallelograms</summary>
        PackingParallelograms = 9,
        /// <summary>QuasiStructuredQuad</summary>
        QuasiStructuredQuad = 11
    }
}
