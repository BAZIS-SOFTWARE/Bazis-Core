using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// IObjectsSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectsSet<T> : ISetInfo,IDictionary<int, T> where T : IModelObject
    {
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="appSet">Добавляемый набор объектов</param>
        /// <remarks>
        /// Номера добавляемых объектов сдвигаются, если пересекаются с номерами объектов набора-приемника.
        /// Пустой набор <paramref name="appSet"/> (как и <c>null</c>) набор-приемник не изменяет.
        /// </remarks>
        void Append(IObjectsSet<T> appSet);
        /// <summary>
        /// ClearWithExistState
        /// </summary>
        void ClearWithExistState();
    }
}
