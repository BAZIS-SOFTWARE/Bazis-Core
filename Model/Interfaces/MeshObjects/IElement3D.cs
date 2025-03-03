using Model.Interfaces.IntegralObject;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.MeshObjects
{
    /// <summary>
    /// IElement3D
    /// </summary>
    public interface IElement3D : ISurfaceElement, IVolume
    {
        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
        /// <param name="dze"></param>
        IntegralPoint CreateIntegrationPoint(double w, double ksi, double eta, double dze);
    }
}
