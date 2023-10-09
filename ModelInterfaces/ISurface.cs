using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ISurface : ISurfaceObject
    {
        Point3D CalcNormal();

        float CalcSquare();

        float GetDistance(Point3D point3D);

        Segment3D[] GetLinesFromSurface(ISurface surface);
    }
}
