using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PostProc
{
    /// <summary>
    /// MergeResults
    /// </summary>
    public class MergeResults : IResultsMerger
    {
        /// <inheritdoc/>
        public void Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, IEnumerable<IResult> results)
        {
            var resNodes = results.Where(x => x.Data.TableName == "nodes");
            var resElements = results.Where(x => x.Data.TableName == "elements");

            if (resNodes.Count() != resElements.Count())
                throw new System.Exception("Наборы результатов узлов и элементов не равны");


            var resBigrams = new List<Tuple<IResult, IResult>>();

            foreach (var item in resNodes)
            {
                var find = resElements.FirstOrDefault(x => x.Time.Equals(item.Time));

                if(find == null)
                    throw new System.Exception("Набор элементов не содержит результатов с таким же индексом");
                
                resBigrams.Add(new Tuple<IResult, IResult>(item, find));
            }

            foreach (var item in resBigrams)
            {
                foreach (DataRow row in item.Item1.Data.Rows)
                {
                    var nodeNumber = (int)row["Индекс"];

                    var value = 0.0f;
                    if (interfaceNodes.ContainsKey(nodeNumber))
                    {
                        foreach (var elemNumber in interfaceNodes[nodeNumber])
                        {
                            value += item.Item2.GetValue(elemNumber, resultDescr);
                        }
                        value /= interfaceNodes[nodeNumber].Count;
                        item.Item1.Data.Rows.Find(nodeNumber)[resultDescr] = value;
                    }
                }
            }
        }       
    }
}
