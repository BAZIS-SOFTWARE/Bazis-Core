using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.FrameCreators
{
    public abstract class FrameCreator : IFrameCreator
    {
        /// <inheritdoc/>
        public Point3D Shifting { get; set; } = new Point3D();
        /// <inheritdoc/>
        public float Rotation { get; set; }


    }
}
