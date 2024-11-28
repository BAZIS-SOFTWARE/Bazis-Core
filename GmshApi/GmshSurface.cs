using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public class GmshSurface
    {
        /// <inheritdoc/>
        public Dictionary<int, GmshVolume> Volumes { get; private set; }
        /// <inheritdoc/>
        public Dictionary<int, GmshCurve> Curves { get; private set; }
        /// <inheritdoc/>
        public int Tag { get; private set; }
        /// <inheritdoc/>
        public bool Visited {  get; set; }
        /// <inheritdoc/>
        public GmshSurface(int tag)
        {
            Tag = tag;
            Volumes = new Dictionary<int, GmshVolume>();
            Curves = new Dictionary<int, GmshCurve>();
        }
        /// <inheritdoc/>
        public IEnumerable<GmshPoint> GetSurfacePoints()
        {
            return Curves.Select(v => v.Value.Points.Select(p => p.Value)).SelectMany(p => p).Distinct();
        }
    }
}
