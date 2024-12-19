using Project.Interfaces;
using Project.Interfaces.IO;
using Results.ResultsData;
using System;
using System.Collections.Generic;
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

            db.CreateTables("nodes", results[0].Data.Tables["nodes"].GetTableSchema());
            db.CreateTables("elements", results[0].Data.Tables["elements"].GetTableSchema());

            if (!append)
                db.Clear();

            foreach (var result in results)
                db.AddData(result);

            db.CloseConnection();
            SaveEvent?.Invoke(this, new SaveEventArgs("Результаты сохранены"));
        }
    }
}
