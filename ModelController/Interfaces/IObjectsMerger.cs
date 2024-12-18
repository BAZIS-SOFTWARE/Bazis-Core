using Model.Interfaces;
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
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers"></param>
        /// <param name="modelObjects"></param>
        /// <returns>
        /// Перечисление слитых объектов
        /// </returns>
        IEnumerable<T> Merge<T>(List<List<int>> numbers, List<T> modelObjects)
                    where T : IModelObject;
    }
}
