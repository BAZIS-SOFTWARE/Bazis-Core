using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IObjectsMerger
    /// </summary>
    public interface IObjectsMerger
    {
        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="numbers"></param>
        /// <param name="modelObjects"></param>
        void Merge(List<List<int>> numbers, List<IModelObject> modelObjects);
    }
}
