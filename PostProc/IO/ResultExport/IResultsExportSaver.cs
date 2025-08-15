using Model.Interfaces;
using ResultDB;
using System;
using System.Collections.Generic;

namespace PostProc.IO.ResultExport
{
    /// <summary>
    /// IResultsPointsSaver
    /// </summary>
    public interface IResultsExportSaver
    {
        /// <summary>
        /// SaveResults
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="result"></param>
        /// <param name="resDescr"></param>
        /// <param name="savePath"></param>
        void SaveResults(IEnumerable<IModelObject> objects, Result result, string resDescr, string savePath);
    }
}
