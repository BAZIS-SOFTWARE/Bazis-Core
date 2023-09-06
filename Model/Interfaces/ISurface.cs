using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ISurface : ISurfaceObject
    {
        Point3D CalcNormal();

        float CalcSquare();

        float GetDistance(Point3D point3D);

        Node this[int ind] { get; }
    }
}
