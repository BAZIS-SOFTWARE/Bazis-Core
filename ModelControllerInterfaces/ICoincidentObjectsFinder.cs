using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// ICoincidentObjectsFinder
    /// </summary>
    public interface ICoincidentObjectsFinder
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <param name="distance"></param>
        /// <returns>
        /// Список групп элементов, находящихся на расстоянии друг от друга меньше заданного.
        /// </returns>
        List<List<int>> Find<T>(List<T> modelObjects, float distance)
            where T : IModelObject;

        /// <summary>
        /// ProgressEvent
        /// </summary>
        event Action<object, float> ProgressEvent;
    }
}
