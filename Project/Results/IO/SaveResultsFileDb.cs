using Project.Interfaces;
using Project.Interfaces.IO;
using Results.ResultsData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Project.Results.IO
{
    /// <summary>
    /// SaveResultsFileDb
    /// </summary>
    public class SaveResultsFileDb : IResultsSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaveEventArgs> SaveEvent;
        /// <inheritdoc/>

        public void Save(List<IResult> results, string path, bool append)
        {
            SaveEvent?.Invoke(this, new SaveEventArgs("Сохранение результатов..."));
            var db = new ResultDataBase(path);
            db.OpenConnection();

            foreach (var item in results)
            {

                db.CreateTables(item.Name, item.GetDataSchema());

                if (!append)
                    db.ClearTable(item.Name);
                AddData(db, item.Time, item.Data);

            }

            db.CloseConnection();
            SaveEvent?.Invoke(this, new SaveEventArgs("Результаты сохранены"));
        }

        /// <summary>
        /// AddData
        /// </summary>
        /// <param name="time"></param>
        /// <param name="table"></param>
        public void AddData(ResultDataBase db, float time, DataTable table)
        {
            var rowCount = table.Rows.Count;
            var colCount = table.Columns.Count;

            var startInd = 0;
            var stopInd = 0;
            var step = 10;
            if (rowCount > 100)
                stopInd = 100;
            else stopInd = 1;

            var columnsStr = CreateColumns(table);
            var valAr = new string[colCount];
            var valStr = string.Empty;
            var queStr = string.Empty;

            if (rowCount > 0)
                while (true)
                {
                    queStr = string.Empty;
                    for (int j = startInd; j < stopInd; j++)
                    {
                        valAr[0] = table.Rows[j][0].ToString();
                        for (int k = 1; k < colCount; k++)
                        {
                            valAr[k] = table.Rows[j][k].ToString();
                        }
                        valStr = string.Join(",", valAr);
                        var timeStr = time.ToString("0.######", CultureInfo.InvariantCulture);
                        valStr = $"{timeStr},{valStr}";

                        queStr += db.CreateAddCommand(table.TableName, columnsStr, valStr);
                        //queStr = queStr + insertCmd + valStr;
                    }

                    db.AddData(queStr);

                    if (stopInd == rowCount)
                        break;

                    startInd = stopInd;
                    stopInd = stopInd + step;
                    if (stopInd > rowCount) stopInd = rowCount;
                }
        }

        private string CreateColumns(DataTable dataTable)
        {
            var columns = dataTable.Columns;
            string[] valStr = new string[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                valStr[i] = columns[i].ColumnName;
            }

            var resVal = String.Join(",", valStr);
            return "Time," + resVal;
        }
    }
}
