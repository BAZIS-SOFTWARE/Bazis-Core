using Model.GmshUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public enum TransfiniteAlgorithm
    {
        Progressive,
        Bump,
        Beta
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
        /// <summary>
        /// Задает расположение узлов на кривой
        /// <param name="number">[In]Количество точек</param>
        /// <param name="ta">[In]Тип расположения точек (Progressive и т.д), числовой параметр</param>
        /// <param name="parameter">[In]Выбранная кривая (может состоять из нескольких последовательно соединённых кривых)</param>
        /// <param name="curve">[In]Кривая или набор кривых</param>
        void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, Curve[] curve);
    }
}
