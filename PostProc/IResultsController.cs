using PostProc.IO;
using PostProc.ScenePresenter.Interfaces;
using Project.Interfaces.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc
{
    /// <summary>
    /// IResultsController
    /// </summary>
    public interface IResultsController
    {
        /// <summary>
        /// ResultsMerger
        /// </summary>
        IResultsMerger ResultsMerger { get; }
        /// <summary>
        /// GradientsCreator
        /// </summary>
        IResultsFieldsCreator ResultsFieldsCreator {get;}
        /// <summary>
        /// ResultsExporter
        /// </summary>
        IResultsExporter ResultsExporter { get; }

        /// <summary>
        /// GridExporter
        /// </summary>
        IGridExporter GridExporter { get; }
    }
}
