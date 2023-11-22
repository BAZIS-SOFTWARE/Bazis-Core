using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ISurface : ISurfaceObject, INormal,ISquare, IEquatable<ISurface>
    {
        float GetDistance(Point3D point3D);

        Segment3D[] GetLinesFromSurface(ISurface surface);

        INode this[int ind] { get; }
    }
}
