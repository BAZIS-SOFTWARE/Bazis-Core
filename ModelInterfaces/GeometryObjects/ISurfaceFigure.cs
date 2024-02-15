using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.GeometryObjects
{
    /// <summary>
    /// ISurfaceFigure
    /// </summary>
    public interface ISurfaceFigure : ISurfaceObject<IGeometrySurface, IGeometryPoint>, ISquare
    {

    }
}
