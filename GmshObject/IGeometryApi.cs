using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshObject
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
        /// Удалить сущность по Id
        /// </summary>
        /// <param name="entityId">[In]Id</param>
        void DeleteEntities(List<ModelObject> entities, bool recursive = false);
        /// <summary>
        /// загрузить геометрию
        /// </summary>
        /// <param name="filePath">[In]Путь до файла</param>
        void Load(string filePath);
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