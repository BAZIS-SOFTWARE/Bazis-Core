using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// ElementKind 3D
    /// </summary>
    public enum ElementKind3D
    {
        /// <summary>
        /// Тетра
        /// </summary>
        Тетра,
        /// <summary>
        /// Гекса
        /// </summary>
        Гекса,
        /// <summary>
        /// Пента
        /// </summary>
        Пента
    }
    public interface IElement3D : ISurfaceElement
    {
        /// <summary>
        /// CalcVolume
        /// </summary>
        /// <returns></returns>
        float CalcVolume();
    }
}
