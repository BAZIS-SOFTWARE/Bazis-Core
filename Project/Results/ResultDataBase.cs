
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Project.Results
{

    public class ResultDataBase
    {
        SQLiteConnection connection;
        SQLiteCommand command;
        /// <summary>
        /// ResultDataBase
        /// </summary>
        /// <param name="fullName"></param>
        public ResultDataBase(string fullName)
        {
            command = new SQLiteCommand();

            if (!File.Exists(fullName))
                SQLiteConnection.CreateFile(fullName);

            connection = new SQLiteConnection("Data Source=" + fullName + ";Version=3;Synchronous=OFF;Journal Mode=WAL;", true);
            command.Connection = connection;

        }
        /// <summary>
        /// CreateTables. Первая колонка всегда имеет тип int и используется 
        /// для хранения номера объекта. Не использовать название "Index" для колонны.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnsName"></param>
        public void CreateTables(string tableName, string[] columnsName)
        {
            var cmdLine = $"CREATE TABLE IF NOT EXISTS[{tableName}] (";

            cmdLine += string.Join(", ", columnsName);

            cmdLine += ")";

            command.CommandText = cmdLine;
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// DeleteTable
        /// </summary>
        /// <param name="tableName"></param>
        public void DeleteTable(string tableName)
        {
            var cmdLine = "DROP TABLE IF EXISTS " + tableName;

            command.CommandText = cmdLine;
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="tableName"></param>
        public void ClearTable(string tableName)
        {           
            command.CommandText = $"DELETE FROM {tableName}";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// CreateAddCommand. Добавление одной строки в таблицу.
        /// Кол-во столбцов и данных должно быть одинаковым.
        /// Название колонн и данные разделены в строке запятой
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string CreateAddCommand(string table, string columns, string values)
        {
            return $"INSERT INTO {table} ({columns}) VALUES ({values});";
        }

        /// <summary>
        /// AddData
        /// </summary>
        /// <param name="queStr"></param>
        /// <returns></returns>

        public int AddData(string queStr)
        {
            command.CommandText = "BEGIN TRANSACTION;" + queStr + "COMMIT;";
            //command.CommandText = strBulder.ToString();
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// GetResults
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>

        public DataTable GetResults(string table, string column, string filter)
        {
            var dataTable = new DataTable(table);
            var cmdStr = $"SELECT {column} FROM {table}{filter}";
            var adapter = new SQLiteDataAdapter(cmdStr, connection);
            adapter.Fill(dataTable);
            return dataTable;
        }
        /// <summary>
        /// OpenConnection
        /// </summary>
        public void OpenConnection()
        {
            connection.Open();           
        }
        /// <summary>
        /// CloseConnection
        /// </summary>
        public void CloseConnection()
        {
                connection.Close();           
        }
    }
}
