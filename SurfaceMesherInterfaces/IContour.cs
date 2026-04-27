using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceMesherInterfaces
{
    public interface IContour
    {
        int NodesCount { get; }

        INode2D GetElementAfter(INode2D node);

        INode2D GetElementBefore(INode2D node);

        int GetContourId(INode2D node);
    }
}