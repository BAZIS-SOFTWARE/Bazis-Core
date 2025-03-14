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

        public void Save(List<Result> results, string path, bool append)
        {
            SaveEvent?.Invoke(this, new SaveEventArgs("Сохранение результатов..."));
            var db = new ResultDataBase(path);
            db.OpenConnection();

            foreach (var item in results)
            {
                var schema = item.Data.GetTableSchema();
                var fullColumnsName = new string[schema.Length + 2];
                fullColumnsName[0] = "Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL";
                fullColumnsName[1] = "Time real DEFAULT(0) NOT NULL";
                fullColumnsName[2] = $"{schema[0]} INTEGER DEFAULT(0) NOT NULL";

                for (int i = 1; i < schema.Length; i++)
                    fullColumnsName[i + 2] = $"{schema[i]} real DEFAULT(0) NOT NULL";

                db.CreateTables(item.Data.TableName, fullColumnsName);

                if (!append)
                    db.ClearTable(item.Data.TableName);
                AddData(db, item);

            }

            db.CloseConnection();
            SaveEvent?.Invoke(this, new SaveEventArgs("Результаты сохранены"));
        }

        /// <summary>
        /// AddData
        /// </summary>
        /// <param name="db"></param>
        /// <param name="result"></param>
        public void AddData(ResultDataBase db, Result result)
        {
            var table = result.Data;
            var rowCount = table.Rows.Count;
            var colCount = table.Columns.Count;

            var startInd = 0;
            var stopInd = 0;
            var step = 10;
            if (rowCount > 100)
                stopInd = 100;
            else stopInd = 1;

            var schemaStr = String.Join(",", result.Data.GetTableSchema());
            var columnsStr = $"Time,{schemaStr}";

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
                        var timeStr = result.Time.ToString("0.######", CultureInfo.InvariantCulture);
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
    }
}
