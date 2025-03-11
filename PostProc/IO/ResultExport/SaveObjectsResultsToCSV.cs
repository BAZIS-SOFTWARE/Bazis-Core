using CsvHelper;
using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc.IO.ResultExport
{
    public class SaveObjectsResultsToCSV : IResultsExportSaver
    {
        public event Action<object, ISaveEventArgs> SaveEvent;

        public void SaveResults(IEnumerable<IModelObject> objects, IResult result, string resDescr, string savePath)
        {
            using (var sw = new StreamWriter(savePath, true, Encoding.Unicode) { AutoFlush = true })
            using (var csvWriter = new CsvWriter(sw, culture:System.Globalization.CultureInfo.InvariantCulture, false))
            {
                foreach (var obj in objects)
                {
                    var value = 0.0f;
                    if (obj is Node node)
                        value = result.GetValue(obj.Number, resDescr);
                    if (obj is IElement element)
                        value = result.GetValue(obj.Number, resDescr);

                    csvWriter.WriteField(obj.Number);
                    csvWriter.WriteField(obj.CalcCentr());
                    csvWriter.WriteField(value);
                    csvWriter.NextRecord();
                }
            }
        }
    }
}
