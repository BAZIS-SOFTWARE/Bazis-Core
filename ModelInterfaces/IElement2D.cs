using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface IElement2D : ISurfaceElement
    {
        float CalcSquare();

        Point3D CalcNormal();
    }
}
