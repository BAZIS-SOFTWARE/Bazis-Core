using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmshApi
{
    /// <summary>
    /// BoundaryLayerOption
    /// </summary>
    public enum BoundaryLayerOption
    {
        /// <summary>
        /// Threshold angle for creating a mesh fan in the boundary layer
        /// </summary>
        AnisoMax,
        /// <summary>
        /// Beta coefficient of the Beta Law
        /// </summary>
        Beta,
        /// <summary>
        /// Use Beta Law instead of geometric progression
        /// </summary>
        BetaLaw,
        /// <summary>
        /// Tags of curves in the geometric model for which a boundary layer is needed
        /// </summary>
        CurvesList,
        /// <summary>
        /// Tags of surfaces in the geometric model where the boundary layer should not be constructed
        /// </summary>
        ExcludedSurfacesList,
        /// <summary>
        /// Tags of points in the geometric model for which a fan is created
        /// </summary>
        FanPointsList,
        /// <summary>
        /// Number of elements in the fan for each fan node. If not present default value Mesh.BoundaryLayerFanElements
        /// </summary>
        FanPointsSizesList,
        /// <summary>
        /// Intersect metrics of all surfaces
        /// </summary>
        IntersectMetrics,
        /// <summary>
        /// Number of Layers in theBeta Law
        /// </summary>
        NbLayers,
        /// <summary>
        /// Tags of points in the geometric model for which a boundary layer ends
        /// </summary>
        PointsList,
        /// <summary>
        /// Generate recombined elements in the boundary layer
        /// </summary>
        Quads,
        /// <summary>
        /// Size ratio between two successive layers
        /// </summary>
        Ratio,
        /// <summary>
        /// Mesh size normal to the curve
        /// </summary>
        Size,
        /// <summary>
        /// Mesh size far from the curves
        /// </summary>
        SizeFar,
        /// <summary>
        /// Mesh size normal to the curve, per point (overwrites Size when defined)
        /// </summary>
        SizesList,
        /// <summary>
        /// Maximal thickness of the boundary layer
        /// </summary>
        Thickness
    }
}
