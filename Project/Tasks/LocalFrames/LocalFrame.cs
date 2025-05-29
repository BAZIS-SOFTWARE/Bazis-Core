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
    public abstract class LocalFrame : ILocalFrame
    {
        /// <inheritdoc/>
        public Point3D Shifting { get; set; } = new Point3D();
        /// <inheritdoc/>
        public float Rotation { get; set; }
/// <inheritdoc/>

        public abstract Frame Build();
    }
}
