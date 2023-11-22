using ModelInterfaces;
using System.Collections.Generic;

namespace Model
{
    public abstract class SurfaceElement : Element, ISurfaceElement
    {
        public SurfaceElement(int number, Node [] nodes, ObjKind objType) : base(number, nodes, objType)
        {
        }

        public abstract int NumberOfSides { get; }

        public abstract IEnumerable<ISurface> GetSurface();
    }
}
