using Model;
using System.Collections.Generic;

namespace GmshApi
{
    public interface IMeshApi
    {
        /// <summary>
        /// Задать\получить размер элементов перед генерацией
        /// </summary>
        double Density { get; set; }
        /// <summary>
        /// сделать больше квадратов на сетке
        /// </summary>
        void QuadrangulateMesh();
        /// <summary>
        /// уплотнить сетку
        /// </summary>
        void RefineMesh();
        void Generate(int dim);
        /// <summary>
        /// получить узлы
        /// </summary>
        /// <returns>узлы</returns>
        IEnumerable<Node> GetNodes();
        /// <summary>
        /// получить линии
        /// </summary>
        /// <returns></returns>
        IEnumerable<ModelObject> GetElement1D();

        /// <summary>
        /// получить плоские элементы
        /// </summary>
        /// <returns>плоские элементы</returns>
        IEnumerable<ModelObject> GetElement2D();
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        IEnumerable<ModelObject> GetSurfaces();
        /// <summary>
        /// получить объёмные элементы
        /// </summary>
        /// <returns>объёмные элементы</returns>
        IEnumerable<ModelObject> GetElement3D();
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 2D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        void ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay);
        /// <summary>
        /// Задает расположение узлов на кривой
        /// <param name="number">[In]Количество точек</param>
        /// <param name="ta">[In]Тип расположения точек (Progressive и т.д), числовой параметр</param>
        /// <param name="parameter">[In]Выбранная кривая (может состоять из нескольких последовательно соединённых кривых)</param>
        /// <param name="curve">[In]Идентификатор кривой</param>
        void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm algorithm, double parameter, int curve);
    }
}