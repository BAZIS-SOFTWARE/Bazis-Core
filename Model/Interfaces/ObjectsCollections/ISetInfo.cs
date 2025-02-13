using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// ISetInfo
    /// </summary>
    public interface ISetInfo : ISetView
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
    }
}
