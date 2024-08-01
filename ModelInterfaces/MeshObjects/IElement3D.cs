using ModelInterfaces.IntegralObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.MeshObjects
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
        void CreateIntegrationPoint(float w, double ksi, double eta, double dze);
    }
}
