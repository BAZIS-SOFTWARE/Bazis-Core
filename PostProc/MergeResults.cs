using Project.Interfaces;
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
            var tableSchema = results.First().Data.Tables["nodes"].Clone();

            foreach (var result in results)
            {
                foreach (DataRow row in result.Data.Tables["nodes"].Rows)
                {
                    var nodeNumber = (int)row["Индекс"];

                    var value = 0.0f;
                    if (interfaceNodes.ContainsKey(nodeNumber))
                    {
                        foreach (var elemNumber in interfaceNodes[nodeNumber])
                        {
                            value += result.GetElementValue(elemNumber, resultDescr);
                        }
                        value /= interfaceNodes[nodeNumber].Count;
                        result.Data.Tables["nodes"].Rows.Find(nodeNumber)[resultDescr] = value;
                    }
                }
            }
        }       
    }
}
