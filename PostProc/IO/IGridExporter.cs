using PostProc.ScenePresenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc.IO
{
    public interface IGridExporter
    {
        /// <summary>
        /// ExportGridSurfaces
        /// </summary>
        /// <param name="figures">Набор поверхностных элементов (фигур) для экспорта сетки</param>
        /// <param name="savePath">Путь (без названия и расширения) сохранения файла экспорта</param>
        /// <param name="format">Формат выходного файла</param>
        void ExportGridSurfaces(IEnumerable<ResultFigure> figures, string savePath, string format);
    }
}
