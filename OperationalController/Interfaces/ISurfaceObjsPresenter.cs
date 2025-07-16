using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController
{
    /// <summary>
    /// IVolumeObjsPresenter
    /// </summary>
    public interface ISurfaceObjsPresenter : IObjsPresenter
    {
        /// <summary>
        /// HideInsideSurfaces
        /// </summary>
        //void HideInsideSurfaces();
        /// <summary>
        /// ShowInsideSurfaces
        /// </summary>
        //void ShowInsideSurfaces();

        /// <summary>
        /// Создать сепараторы, по разнице текущего и предыдущего значения в массиве можно определить тип элемента(кол-во треугольников)
        /// </summary>
        /// <returns>Массив сепараторов</returns>
        int[] CreateSeparators();
    }
}
