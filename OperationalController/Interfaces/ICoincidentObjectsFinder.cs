using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController
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

    }
}
