using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IElement2D : ISurfaceElement
    {
        float CalcSquare();
    }
}
