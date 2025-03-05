using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
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
        void Merge(List<List<int>> numbers, IObjectsSet<Node> modelObjects);
    }
}
