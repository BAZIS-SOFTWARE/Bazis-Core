using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IFreeNodesFinder
    /// </summary>
    public interface IFreeNodesFinder
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objsData"></param>
        /// <returns></returns>
        IEnumerable<int> Find(IObjectsData objsData);
    }
}
