using System;
using System.Collections.Generic;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public enum MeshAlgorithm
    {
        /// <inheritdoc/>
        MeshAdapt = 1,
        /// <inheritdoc/>
        Automatic = 2,
        /// <inheritdoc/>
        InitialMeshOnly = 3,
        /// <inheritdoc/>
        Delaunay = 5,
        /// <inheritdoc/>
        FrontalDelaunay = 6,
        /// <inheritdoc/>
        BAMG = 7,
        /// <inheritdoc/>
        FrontalDelaunayQuads = 8,
        /// <inheritdoc/>
        PackingOfParallelograms = 9,
        /// <inheritdoc/>
        QuasiStructuredQuad = 11
    }

    /// <inheritdoc/>
    public enum MeshAlgorithm3D
    {
        /// <inheritdoc/>
        Delaunay = 1,
        /// <inheritdoc/>
        InitialMeshOnly = 3,
        /// <inheritdoc/>
        Frontal = 4,
        /// <inheritdoc/>
        MMG3D = 7,
        /// <inheritdoc/>
        RTree = 9,
        /// <inheritdoc/>
        HXT = 10
    }
}
