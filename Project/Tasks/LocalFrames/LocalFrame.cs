using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.FrameCreators
{
    /// <summary>
    /// LocalFrame
    /// </summary>
    public abstract class LocalFrame
    {
        /// <inheritdoc/>
        public Point3D Shifting { get; set; } = new Point3D();
        /// <inheritdoc/>
        public float Rotation { get; set; }
        /// <summary>
        /// Frame
        /// </summary>
        internal Frame frame;
/// <inheritdoc/>

        public abstract Frame Frame { get;}
        /// <summary>
        /// CalcPosition
        /// </summary>
        public abstract void CalcPosition();
    }
}
