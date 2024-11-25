using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GmshApi
{
    /// <inheritdoc/>
    public class GmshPoint
    {
        /// <inheritdoc/>
        public Dictionary<int, GmshCurve> Curves { get; private set; }
        /// <inheritdoc/>
        public int Tag {  get; private set; }
        /// <inheritdoc/>
        public Vector3 Coords { get; set; }
        /// <inheritdoc/>
        public GmshPoint(int tag, Vector3 coords)
        {
            Tag = tag;
            Coords = coords;
        }
        /// <inheritdoc/>
        public GmshPoint(int tag)
        {
            Tag = tag;
            Curves = new Dictionary<int, GmshCurve>();
        }
    }
}
