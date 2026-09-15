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
        /// Добавляет объекты из другого набора в текущий.
        /// </summary>
        /// <param name="appSet">Добавляемый набор объектов</param>
        /// <returns>Имя набора-приёмника, в том числе при добавлении пустого набора.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="appSet"/> равен <c>null</c>.</exception>
        /// <remarks>
        /// Номера добавляемых объектов сдвигаются, если пересекаются с номерами объектов набора-приемника.
        /// Пустой набор <paramref name="appSet"/> набор-приемник не изменяет.
        /// </remarks>
        string Append(IObjectsSet<T> appSet);
        /// <summary>
        /// ClearWithExistState
        /// </summary>
        void ClearWithExistState();
    }
}
