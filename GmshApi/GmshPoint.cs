using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

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
        /// <inheritdoc/>
        public static int FindSurfaceByPoints(List<GmshPoint> points, int srcPointsCount)
        {
            var curves = points.Select(v => v.Curves.Values.Select(c => c)).SelectMany(v => v).Distinct();
            return curves.Select(v => new { CurveId = v.Tag, Surfs = v.Surfaces.Values.Select(s => s) })
                          .SelectMany(v => v.Surfs, (d, s) => new { d.CurveId, Surf = s })
                          .GroupBy(v => v.Surf.Tag, e => e.CurveId)
                          .ToDictionary(k => k.Key, v => v.Select(t => t).ToList())
                          .Where(v => v.Value.Count == srcPointsCount)
                          .Select(v => v.Key)
                          .FirstOrDefault();
        }
    }
}
