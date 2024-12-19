using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Results;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Results.IO.ResultExport
{
    public class SaveObjectsResultsToTXT : IResultsExportSaver
    {
        public event Action<object, ISaveEventArgs> SaveEvent;

        public void SaveResults(IEnumerable<IModelObject> objects, IResult result, string resDescr, string savePath)
        {
            using (var sw = new StreamWriter(savePath, true, Encoding.Unicode) { AutoFlush = true })
            {
                foreach (var obj in objects)
                {
                    var value = 0.0f;
                    if (obj is Node node)
                        value = result.GetNodeValue(obj.Number, resDescr);
                    if (obj is IElement element)
                        value = result.GetElementValue(obj.Number, resDescr);
                    sw.WriteLine($"{obj.Number} {obj.CalcCentr()} {value}");
                }
                sw.WriteLine($"");
            }
        }
    }
}
