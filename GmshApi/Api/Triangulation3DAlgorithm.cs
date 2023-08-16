namespace GmshApi
{
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
}
