using Model;
using System.Collections.Generic;

namespace GmshApi
{
    public interface IGeometryApi
    {
        /// <summary>
        /// установить шаг установки точек для визуализации кривых
        /// </summary>
        int CurveSteps { get; set; }
        /// <summary>
        /// Получить\Установить Id
        /// </summary>
        ulong Id { get; set; }
        /// <summary>
        /// // сгенерировать сетку Элементы2D, и т. д.
        /// </summary>
        /// <param name="dim"></param>
        IMeshApi Generate(int dim);
        /// <summary>
        /// получить контрольные точки
        /// </summary>
        /// <returns>узлы</returns>
        IEnumerable<Node> GetControlPoints();
        /// <summary>
        /// получить кривые
        /// </summary>
        /// <returns>узлы</returns>
        IEnumerable<Curve> GetСurves();
    }
}