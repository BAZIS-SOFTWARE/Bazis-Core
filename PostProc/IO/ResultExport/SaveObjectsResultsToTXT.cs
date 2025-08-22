using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using ResultDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc.IO.ResultExport
{
    public class SaveObjectsResultsToTXT : IResultsExportSaver
    {

        public void SaveResults(IEnumerable<IModelObject> objs, Result result, string resDescr, string savePath)
        {
            using (var sw = new StreamWriter(savePath, true, Encoding.Unicode) { AutoFlush = true })
            {
                foreach (var obj in objs)
                {
                    var value = 0.0f;
                    if (obj is Node node)
                        value = result.GetValue(0, obj.Number, resDescr);
                    if (obj is IElement element)
                        value = result.GetValue(1, obj.Number, resDescr);

                    sw.WriteLine($"{obj.Number} {obj.CalcCentr()} {value}");
                }
                sw.WriteLine($"");
            }
        }
    }
}
