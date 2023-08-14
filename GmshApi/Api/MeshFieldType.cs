using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmshApi.Api
{
    /// <summary>
    /// Типы полей
    /// </summary>
    public enum MeshFieldType
    {
        AttractorAnisoCurve,
        AutomaticMeshSizeField,
        Ball,
        Box,
        BoundaryLayer,
        Constant,
        Curvature,
        Cylinder,
        Distance,
        Extend,
        Frustum,
        Gradient,
        IntersectAniso,
        Laplacian,
        LonLat,
        MathEval,
        MathEvalAniso,
        Max,
        MaxEigenHessian,
        Mean,
        Min,
        MinAniso,
        Octree,
        Param,
        PostView,
        Restrict,
        Structured,
        Threshold
    }

    public enum DisplayFieldType
    {
        BackgroundMesh,
        BoundaryLayer
    }
}
