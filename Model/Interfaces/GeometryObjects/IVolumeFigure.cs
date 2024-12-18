using Model.GeometryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.GeometryObjects
{
    /// <summary>
    /// IVolumeFigure
    /// </summary>
    public interface IVolumeFigure : ISurfaceObject<GeometrySurface, GeometryPoint>, IVolume
    {

    }
}
