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
        /// <inheritdoc/>
        public MergeResults ResultsMerger { get; } = new MergeResults();

        /// <inheritdoc/>
        public IResultsFieldsCreator ResultsFieldsCreator { get; } = new ResultsFieldsCreator();

        /// <inheritdoc/>
        public IResultsExporter ResultsExporter { get; }  = new ResultsExporter();

        /// <inheritdoc/>
        public IGridExporter GridExporter { get; } = new GridExporter();
    }
}
