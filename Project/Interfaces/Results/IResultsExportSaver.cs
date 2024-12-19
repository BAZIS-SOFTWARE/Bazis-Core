using Model.Interfaces;
using Project.Interfaces.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces.Results
{
    /// <summary>
    /// IResultsPointsSaver
    /// </summary>
    public interface IResultsExportSaver
    {
        /// <summary>
        /// SaveEvent
        /// </summary>
        event Action<object, ISaveEventArgs> SaveEvent;
        /// <summary>
        /// SaveResults
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="result"></param>
        /// <param name="resDescr"></param>
        /// <param name="savePath"></param>
        void SaveResults(IEnumerable<IModelObject> objects, IResult result, string resDescr, string savePath);
    }
}
