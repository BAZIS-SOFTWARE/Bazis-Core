using Geometry;
using ModelInterfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Model.MeshObjects
{
    /// <summary>
    /// IntegralPoint
    /// </summary>
    public class IntegralPoint : IIntegralPoint
    {
        /// <inheritdoc/>
        public float Weigt { get; set; }
/// <inheritdoc/>

        public float _x { get; set; }
/// <inheritdoc/>

        public float _y { get; set; }
/// <inheritdoc/>

        public float _z { get; set; }
    }
}