using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces.Results
{
    /// <summary>
    /// IResultsExporter
    /// </summary>
    public interface IResultsExporter
    {
        /// <summary>
        /// ExportResultSurfaces. Сохраняет результаты по элементам
        /// </summary>
        /// <param name="objects">Объекты материнской сетки (содержат координаты)</param>
        /// <param name="result">Значения результатов</param>
        /// <param name="resDesc">Описание результата</param>
        /// <param name="savePath">Путь (без названия и расширения) сохранения файла экспорта</param>
        /// <param name="format">Формат выходного файла</param>
        void ExportObjectsResults(IEnumerable<IModelObject> objects, IResult result,string resDesc, string savePath, string format);

        /// <summary>
        /// ExportResultPoints. Сохраняет результаты по узлам
        /// </summary>
        /// <param name="nodes">Набор узлов для экспорта результатов</param>
        /// <param name="taskKind">тип результатов</param>
        /// <param name="resDesc">описание результата</param>
        /// <param name="savePath">Путь (без названия и расширения) сохранения файла экспорта</param>
        /// <param name="format">Формат выходного файла</param>
        //void ExportResultNodes(IEnumerable<IModelObject> nodes, string taskKind, string resDesc, string savePath, string format);
    }
}
