using PostProc.ScenePresenter;
using System;
using System.Collections.Generic;

namespace PostProc.IO.GridExport
{
    public interface IGridSurfacesSaver
    {
        /// <summary>
        /// SaveEvent
        /// </summary>
        event Action<object, string> SaveEvent;
        /// <summary>
        /// SaveResults
        /// </summary>
        /// <param name="figures"></param>
        /// <param name="savePath"></param>
        void SaveGrid(IEnumerable<ResultFigure> figures, string savePath);
    }
}
