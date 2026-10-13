using Geometry;
using Model.GeometryObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции чтения геометрической модели.
    /// </summary>
    public interface IGeometryQueries
    {
        IEnumerable<IModelObject> GetAdjacentGeometryObjects(IModelObject obj, int direction);
        /// <summary>Возвращает номера смежных геометрических сущностей верхнего и нижнего уровня.</summary>
        Tuple<int[], int[]> GetAdjacentGeometryObjects(int dim, int number);
        GeometryPoint GetModelPoint(int objNumber);
        Curve GetModelCurve(int objNumber);
        SurfaceFigure GetModelSurface(int objNumber);
        IEnumerable<VolumeFigure> GetModelVolumes();
        /// <summary>
        /// Возвращает номера сущностей исходной размерности, связанных в заданной топологической области.
        /// </summary>
        HashSet<int> SelectByScope(int startDim, IEnumerable<int> selectedNumbers, int targetDim);
        double[] GetGeoObjPoints(int dim, int tag, double[] parametricCoord);
        double[] GetParametrization(int dim, int tag, double[] coordinates);
        Tuple<double[], double[]> GetParametrizationBounds(int dim, int tag);
        double[] GetSurfaceNormal(int surfaceTag, double[] parametricCoordinates);
        string GetType(int dim, int tag);
        (double X, double Y, double Z) GetCenterOfMass(int dim, int tag);
        string GetGeometryFilePath();
        int[] GetBoundary(int[] dimTags, bool combined = true, bool oriented = true, bool recursive = false);
        double GetCurveLength(int curveTag);
        string GetGeometryLastError();
    }
}
