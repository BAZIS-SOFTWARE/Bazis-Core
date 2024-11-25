using System;
using System.Collections.Generic;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public class GmshCurve
    {
        /// <inheritdoc/>
        public Dictionary<int, GmshSurface> Surfaces { get; private set; }
        /// <inheritdoc/>
        public Dictionary<int, GmshPoint> Points { get; private set; }
        /// <inheritdoc/>
        public int Tag { get; private set; }
        /// <inheritdoc/>
        public GmshCurve(int tag)
        {
            Tag = tag;
            Surfaces = new Dictionary<int, GmshSurface>();
            Points = new Dictionary<int, GmshPoint>();
        }
    }
}
