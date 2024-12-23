using Model.Interfaces;
using Project.Interfaces.Results;
using Project.Interfaces;
using System;
using System.Collections.Generic;
using PostProc.IO.ResultExport;

namespace PostProc.IO
{
    /// <summary>
    /// ResultExporter
    /// </summary>
    public class ResultsExporter : IResultsExporter
    {
/// <inheritdoc/>

        public void ExportObjectsResults(IEnumerable<IModelObject> objs, IResult result, string resDesc, string savePath, string format)
        {
            IResultsExportSaver saver;
            if (format.Equals("txt"))
                saver = new SaveObjectsResultsToTXT();
            else if (format.Equals("csv"))
                saver = new SaveObjectsResultsToCSV();
            else throw new Exception($"Выбранный формат {format} не реализовн, либо допущена ошибка при вводе");
            
            
            
            saver.SaveResults(objs, result, resDesc, savePath);
        }

        /// <summary>
        /// ExportResultPoints
        /// </summary>
        /// <param name="points"></param>
        /// <param name="result"></param>
        /// <param name="resDesc"></param>
        /// <param name="savePath"></param>
        /// <param name="format"></param>
        //public void ExportResultNodes(IEnumerable<IModelObject> points, string taskKind, string resDesc, string savePath, string format)
        //{
        //    IResultsExportSaver saver;
        //    if (format.Equals("txt"))
        //        saver = new SaveResultsNodesToTXT();
        //    else if (format.Equals("csv"))
        //        saver = new SaveResultsNodesToCSV();
        //    else throw new Exception($"Выбранный формат {format} не реализовн, либо допущена ошибка при вводе");
        //    saver.SaveResults(points, taskKind, resDesc, savePath);
        //}
    }
}
