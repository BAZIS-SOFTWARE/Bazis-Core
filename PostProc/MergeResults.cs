using Project.Interfaces;
using Project.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PostProc
{
    /// <summary>
    /// MergeResults
    /// </summary>
    public class MergeResults
    {
        /// <inheritdoc/>
        public void Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, Result res)
        {

            if (res.Data.Tables.Count < 1)
                throw new System.Exception("Результаты должны содержать две таблицы для усреднения");

            if (res.Data.Tables[0].Columns[0].DataType != typeof(int))
                throw new System.Exception("Неверный тип данных колонки с номерами узлов");

            if (res.Data.Tables[1].Columns[0].DataType != typeof(int))
                throw new System.Exception("Неверный тип данных колонки с номерами элементов");


            foreach (DataRow row in res.Data.Tables[0].Rows)
            {
                var nodeNumber = (int)row[0];

                var value = 0.0f;
                if (interfaceNodes.ContainsKey(nodeNumber))
                {
                    foreach (var elemNumber in interfaceNodes[nodeNumber])
                    {
                        value += res.GetValue(1,elemNumber, resultDescr);
                    }
                    value /= interfaceNodes[nodeNumber].Count;
                    res.Data.Tables[0].Rows.Find(nodeNumber)[resultDescr] = value;
                }

            }
        }       
    }
}
