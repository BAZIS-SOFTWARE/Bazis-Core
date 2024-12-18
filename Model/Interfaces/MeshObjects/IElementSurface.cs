using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.MeshObjects
{
    /// <summary>
    /// IElementSurface
    /// </summary>
    public interface IElementSurface : ISurface<Node>
    {
        /// <summary>
        /// ElementNumber
        /// </summary>
        int ElementNumber { get; }
    }
}
