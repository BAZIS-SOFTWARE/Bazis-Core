using System;
using System.Collections.Generic;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public class GmshVolume
    {
        /// <inheritdoc/>
        public Dictionary<int, GmshSurface> Surfaces { get; private set; }
        /// <inheritdoc/>
        public int Tag { get; private set; }
        /// <inheritdoc/>
        public GmshVolume(int tag)
        {
            Tag = tag;
            Surfaces = new Dictionary<int, GmshSurface>();
        }

    }
}
