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
        public void Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, Result ndRes, Result elRes)
        {
            if(ndRes.Data.Columns[0].DataType != typeof(int))
                throw new System.Exception("Неверный тип данных колонки с номерами узлов");

            if (elRes.Data.Columns[0].DataType != typeof(int))
                throw new System.Exception("Неверный тип данных колонки с номерами узлов");

            if (!elRes.Time.Equals(ndRes.Time))
                throw new System.Exception("Время в результатах не совпадает");


            foreach (DataRow row in ndRes.Data.Rows)
            {
                var nodeNumber = (int)row[0];

                var value = 0.0f;
                if (interfaceNodes.ContainsKey(nodeNumber))
                {
                    foreach (var elemNumber in interfaceNodes[nodeNumber])
                    {
                        value += elRes.GetValue(elemNumber, resultDescr);
                    }
                    value /= interfaceNodes[nodeNumber].Count;
                    ndRes.Data.Rows.Find(nodeNumber)[resultDescr] = value;
                }

            }
        }       
    }
}
