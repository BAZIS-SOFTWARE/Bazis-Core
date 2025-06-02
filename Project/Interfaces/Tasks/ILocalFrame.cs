using Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// ILocalFrame
    /// </summary>
    public interface ILocalFrame
    {
        /// <summary>
        /// Shifting
        /// </summary>
        Point3D Shifting { get; set; }
        /// <summary>
        /// Rotation
        /// </summary>
        float Rotation { get; set; }
        /// <summary>
        /// Frame
        /// </summary>
        /// <returns></returns>
        Frame Frame { get; set; }
    }
}
