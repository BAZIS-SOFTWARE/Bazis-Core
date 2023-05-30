using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class SurfaceElement : Element, ISurfaceElement
    {
        public SurfaceElement(int number, Node [] nodes, ObjKind objType) : base(number, nodes, objType)
        {
        }

        public abstract int NumberOfSides { get; }

        public abstract IEnumerable<Surface> GetSurface();
    }
}
