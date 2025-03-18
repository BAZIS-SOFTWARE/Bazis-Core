using PostProc.IO;
using PostProc.ScenePresenter;
using PostProc.ScenePresenter.Interfaces;
using Project.Interfaces.Results;
using Project.Results.IO;

namespace PostProc
{
    /// <summary>
    /// ResultsController
    /// </summary>
    public class PostProcController
    {
        /// <summary>
        /// ResultDBPath
        /// </summary>
        public string ResultDBPath { get; set; }
        /// <summary>
        /// ResultsMerger
        /// </summary>
        public MergeResults ResultsMerger { get; } = new MergeResults();

        /// <summary>
        /// ResultsFieldsCreator
        /// </summary>
        public ResultsFieldsCreator ResultsFieldsCreator { get; } = new ResultsFieldsCreator();

        /// <summary>
        /// ResultsExporter
        /// </summary>
        public IResultsExporter ResultsExporter { get; }  = new ResultsExporter();

        /// <summary>
        /// GridExporter
        /// </summary>
        public IGridExporter GridExporter { get; } = new GridExporter();
    }
}
