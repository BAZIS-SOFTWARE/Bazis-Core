using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IVolumeObjsPresenter
    /// </summary>
    public interface IVolumeObjsPresenter : IObjsPresenter
    {
        /// <summary>
        /// HideInsideSurfaces
        /// </summary>
        void HideInsideSurfaces();
        /// <summary>
        /// ShowInsideSurfaces
        /// </summary>
        void ShowInsideSurfaces();
    }
}
