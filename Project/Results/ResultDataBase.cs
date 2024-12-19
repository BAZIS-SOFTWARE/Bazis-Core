
using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace Project.Results
{

    public class ResultDataBase
    {
        SQLiteConnection connection;
        SQLiteCommand command;

        public ResultDataBase(string fullName)
        {
            command = new SQLiteCommand();

            if (!File.Exists(fullName))
                SQLiteConnection.CreateFile(fullName);

            connection = new SQLiteConnection("Data Source=" + fullName + ";Version=3;Synchronous=OFF;Journal Mode=WAL;", true);
            command.Connection = connection;

        }

        public void CreateTables(string tableName, List<string> columnsName)
        {
            var cmdLine = "CREATE TABLE IF NOT EXISTS[" + tableName + "] " +
                "([Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL " +
                ", [Time] real DEFAULT(0) NOT NULL" +
                ", [Индекс] INTEGER DEFAULT(0) NOT NULL";

            foreach (var columnName in columnsName)
                cmdLine += string.Format(", [{0}] real DEFAULT(0) NOT NULL", columnName);

            cmdLine += ")";

            command.CommandText = cmdLine;
            command.ExecuteNonQuery();
        }

        public void DeleteTable(string tableName)
        {
            var cmdLine = "DROP TABLE IF EXISTS " + tableName;

            command.CommandText = cmdLine;
            command.ExecuteNonQuery();
        }   

        public void AddData(IResult results)
        {
                AddDataKind(results.Time, results.Data);           
        }

        public void Clear()
        {           
            command.CommandText = "DELETE FROM nodes";
            command.ExecuteNonQuery();
            command.CommandText = "DELETE FROM elements";
            command.ExecuteNonQuery();
        }     

        private void AddDataKind(float time, DataSet dataSet)
        {
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                var dataTable = dataSet.Tables[i];
                var rowCount = dataTable.Rows.Count;
                var colCount = dataTable.Columns.Count;

                var startInd = 0;
                var stopInd = 0;
                var step = 10;
                if (rowCount > 100)
                    stopInd = 100;
                else stopInd = 1;

                var insertCmd = CreateAddCommand(dataTable);
                var valAr = new string[colCount];
                var valStr = string.Empty;
                var queStr = string.Empty;
    
                if (rowCount > 0)
                    while (true)
                    {
                        queStr = string.Empty;
                        for (int j = startInd; j < stopInd; j++)
                        {
                            valAr[0] = dataTable.Rows[j][0].ToString();
                            for (int k = 1; k < colCount; k++)
                            {
                                //var val = Convert.ToSingle(dataSet.Tables[i].Rows[j][k], CultureInfo.InvariantCulture);
                                //valAr[k] = val.ToString(CultureInfo.InvariantCulture); 
                                valAr[k] = dataSet.Tables[i].Rows[j][k].ToString();
                            }
                            valStr = string.Join(",", valAr);
                            valStr = string.Format("( {0},{1} );", time.ToString("0.######",
                            CultureInfo.InvariantCulture), valStr);
                            queStr = queStr + insertCmd + valStr;
                        }
                        //strBulder.Append("COMMIT;");
                        command.CommandText = "BEGIN TRANSACTION;" + queStr + "COMMIT;";
                        //command.CommandText = strBulder.ToString();
                        command.ExecuteNonQuery();

                        if (stopInd == rowCount)
                            break;

                        startInd = stopInd;
                        stopInd = stopInd + step;
                        if (stopInd > rowCount) stopInd = rowCount;
                    }
            }
        }

        private string CreateAddCommand(DataTable dataTable)
        {
            var columns = dataTable.Columns;
            string[] valStr = new string[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                valStr[i] = columns[i].ColumnName;   
            }

            var resVal = String.Join(",", valStr);
            resVal = "Time," + resVal;

            return String.Format("INSERT INTO {0} ({1}) VALUES ", dataTable.TableName, resVal);
        }



        public DataTable GetResults(string table, string column)
        {
            var dataTable = new DataTable(table);
            var cmdStr = "SELECT " + column + " FROM " + table;
            var adapter = new SQLiteDataAdapter(cmdStr, connection);
            adapter.Fill(dataTable);
            return dataTable;
        }

        public void OpenConnection()
        {
            connection.Open();           
        }

        public void CloseConnection()
        {
                connection.Close();           
        }
    }
}
