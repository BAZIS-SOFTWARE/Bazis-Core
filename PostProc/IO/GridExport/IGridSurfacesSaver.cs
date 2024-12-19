using PostProc.ScenePresenter;
using Project.Interfaces.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc.IO.GridExport
{
    public interface IGridSurfacesSaver
    {
        /// <summary>
        /// SaveEvent
        /// </summary>
        event Action<object, ISaveEventArgs> SaveEvent;
        /// <summary>
        /// SaveResults
        /// </summary>
        /// <param name="figures"></param>
        /// <param name="savePath"></param>
        void SaveGrid(IEnumerable<ResultFigure> figures, string savePath);
    }
}
