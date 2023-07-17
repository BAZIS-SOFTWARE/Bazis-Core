using Model.GmshUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshObject
{
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
