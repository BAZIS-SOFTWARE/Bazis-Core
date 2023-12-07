using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface IElement3D : ISurfaceElement
    {
        /// <summary>
        /// CalcVolume
        /// </summary>
        /// <returns></returns>
        float CalcVolume();
    }
}
