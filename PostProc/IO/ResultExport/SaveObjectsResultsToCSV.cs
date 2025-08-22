using CsvHelper;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using ResultDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PostProc.IO.ResultExport
{
    public class SaveObjectsResultsToCSV : IResultsExportSaver
    {
        public void SaveResults(IEnumerable<IModelObject> objects, Result result, string resDescr, string savePath)
        {
            using (var sw = new StreamWriter(savePath, true, Encoding.Unicode) { AutoFlush = true })
            using (var csvWriter = new CsvWriter(sw, culture:System.Globalization.CultureInfo.InvariantCulture, false))
            {
                foreach (var obj in objects)
                {
                    var value = 0.0f;
                    if (obj is Node node)
                        value = result.GetValue(0,obj.Number, resDescr);
                    if (obj is IElement element)
                        value = result.GetValue(1,obj.Number, resDescr);

                    csvWriter.WriteField(obj.Number);
                    csvWriter.WriteField(obj.CalcCentr());
                    csvWriter.WriteField(value);
                    csvWriter.NextRecord();
                }
            }
        }
    }
}
