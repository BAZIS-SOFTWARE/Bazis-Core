using Project.Interfaces;
using Project.Interfaces.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Project.Results.IO
{
    public class SaveResultsFileTextFormat : IResultsSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaveEventArgs> SaveEvent;

        public void Save(List<Result> results, string path, bool append)
        {
            using (var sw = new StreamWriter(path, append, System.Text.Encoding.Unicode) { AutoFlush = true })
            {
                sw.WriteLine("Результаты");

                WriteResultsDescription(sw, results[0]);


                foreach (var result in results)
                {
                    WriteResultsValue(result.Data, result.Time, result.TaskKind, sw);
                }
                sw.WriteLine("#Результаты");
            }
        }

        private void WriteResultsValue(DataSet dataSet, float time, TaskKind taskKind, StreamWriter sw)
        {
            foreach (DataTable item in dataSet.Tables)
            {
                sw.WriteLine("Время " + time.ToString());
                sw.WriteLine(item.TableName);

                DataRowCollection rows;

                rows = item.Rows;

                var rowsCount = rows.Count;

                for (int j = 0; j < rowsCount; j++)
                    sw.WriteLine(string.Join(" ", rows[j].ItemArray));

                sw.WriteLine($"#{item.TableName}");
                sw.WriteLine("#Время " + time.ToString());
            }  
        }

        private void WriteResultsDescription(StreamWriter sw, Result result)
        {
            var str = new List<string>();

            str.Add(result.TaskKind.ToString());

            foreach (DataTable table in result.Data.Tables)
            {
                str.Add(table.TableName);
                foreach (DataColumn col in table.Columns)
                {
                    str.Add(col.ColumnName);
                }
            }

            sw.WriteLine(string.Join(" ", str.ToArray()));
            str.Clear();
        }
    }
}
