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
        //InitialMeshOnly = 3, Не корректно работает в Gmsh
        /// <summary>Delaunay</summary>
        Delaunay = 5,
        /// <summary>FrontalDelaunay</summary>
        FrontalDelaunay = 6,
        //BAMG = 7, //Эксперементальный - не корректно работает в Gmsh
        /// <summary>FrontalDelaunayQuad</summary>
        FrontalDelaunayQuad = 8,
        //PackingParallelograms = 9,//Эксперементальный - не корректно работает в Gmsh
        //QuasiStructuredQuad = 11 //Эксперементальный - не корректно работает в Gmsh
    }
}
