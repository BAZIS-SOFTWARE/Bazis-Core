using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Project.Results.IO
{
    public class CSVToDataTableConverter
    {
        /// <summary>
        /// ConvertCSVtoDataTable
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(';');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
