using Geometry;
using Model.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// ISelectionHelper
    /// </summary>
    public interface ISelectionHelper
    {
        /// <summary>
        /// Выбор всех элементов, лежащих в одной плоскости с заданным элементом, 
        /// и углом между плоскостями, не большим чем заданный угол
        /// </summary>
        /// <returns>
        /// Список номеров выбранных элементов
        /// </returns>
        List<int> SelectE2DInPlane(IObjectsData objectsData, float angleInGradus, int startNumber, Color color);
        /// <summary>
        /// SelectNodeInPlane
        /// </summary>
        /// <param name="objectsData">
        /// <param name="plane"></param>
        /// <param name="color"></param>
        List<int> SelectNodeInPlane(IObjectsData objectsData, Plane plane, Color color);

        /// <summary>
        /// Выбор последовательности узлов по заданному напрвлению. Направление задаётся номерами двух смежных узлов.
        /// </summary>
        /// <param name="objectsData">
        /// <param name="angleInGradus">Максимальный угол в градусах, под которым будет выделение</param>
        /// <param name="numbFirst">Номер первого узла</param>
        /// <param name="numbSecond">Номер второго узла</param>
        /// <param name="color">Цвет выделения</param>
        /// <returns>
        /// Номера выделенных узлов
        /// </returns>
        List<int> SelectNodeInDirection(IObjectsData objectsData, float angleInGradus, int numbFirst, int numbSecond, Color color);
    }
}
