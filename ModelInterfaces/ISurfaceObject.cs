using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ISurfaceObject : INodeObject
    {
        IEnumerable<ISurface> GetSurface();

        /// <summary>
        /// NumberOfSides
        /// </summary>
        int NumberOfSides { get; }
    }
}
