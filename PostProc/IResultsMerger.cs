using Project.Interfaces;
using System.Collections.Generic;

namespace PostProc
{
    public interface IResultsMerger
    {
        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="interfaceNodes"></param>
        /// <param name="resultDescr"></param>
        /// <param name="results"></param>
        void Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, IEnumerable<IResult> results);
    }
}