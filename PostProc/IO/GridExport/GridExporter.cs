using PostProc.ScenePresenter;
using System;
using System.Collections.Generic;

namespace PostProc.IO.GridExport
{
    /// <summary>
    /// 
    /// </summary>
    public class GridExporter : IGridExporter
    {
        /// <summary>
        /// Экспортирует результирующие поверхности в виде сетки из узлов и 2D элементов
        /// </summary>
        /// <param name="figures">фигуры из результирующих элементов</param>
        /// <param name="savePath">путь сохранения (без названия и расширения)</param>
        /// <param name="format">формат экспортируемого файла</param>
        /// <exception cref="Exception">выбранный формат сохранения не реализован</exception>
        public void ExportGridSurfaces(IEnumerable<ResultFigure> figures, string savePath, string format)
        {
            IGridSurfacesSaver saver;
            if (format.ToLower() == ".bpf")
                saver = new SaveGridSurfacesToBPF();
            else if (format.ToLower() == ".stl-bin")
                saver = new SaveGridSurfacesToBinSTL();
            else if (format.ToLower() == ".stl-text")
                saver = new SaveGridSurfacesToTxtSTL();
            else
                throw new Exception("Выбранный формат сохранения некоректен или не реализован для программы");

            saver.SaveGrid(figures, savePath);
        }
    }
}
