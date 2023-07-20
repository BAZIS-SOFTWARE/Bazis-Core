using System.Collections.Generic;

namespace Model.GmshObject
{
    public interface IMeshApi
    {
        double Density { get; set; }
        /// <summary>
        /// // сгенерировать сетку Элементы2D, и т. д.
        /// </summary>
        /// <param name="dim"></param>
        void Generate(int dim);

        /// <summary>
        /// сделать больше квадратов на сетке
        /// </summary>
        void QuadrangulateMesh();
        /// <summary>
        /// уплотнить сетку
        /// </summary>
        void RefineMesh();
        /// <summary>
        /// получить узлы
        /// </summary>
        /// <returns>узлы</returns>
        IEnumerable<Node> GetNodes();
        /// <summary>
        /// получить линии
        /// </summary>
        /// <returns></returns>
        IEnumerable<Model.ModelObject> GetElement1D();

        /// <summary>
        /// получить плоские элементы
        /// </summary>
        /// <returns>плоские элементы</returns>
        IEnumerable<Model.ModelObject> GetElement2D();
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        IEnumerable<Model.ModelObject> GetSurfaces();
        /// <summary>
        /// получить объёмные элементы
        /// </summary>
        /// <returns>объёмные элементы</returns>
        IEnumerable<Model.ModelObject> GetElement3D();
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 2D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        void ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay);
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 3D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        void ChangeTriangulationAlgorithm3D(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay);
        /// <summary>
        /// Задает расположение узлов на кривой
        /// <param name="number">[In]Количество точек</param>
        /// <param name="ta">[In]Тип расположения точек (Progressive и т.д), числовой параметр</param>
        /// <param name="parameter">[In]Выбранная кривая (может состоять из нескольких последовательно соединённых кривых)</param>
        /// <param name="curve">[In]Идентификатор кривой</param>
        void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, int curve);
    }
}
