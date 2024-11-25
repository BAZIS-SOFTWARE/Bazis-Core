using ModelControllerInterfaces.GmshController;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public class GmshEntitiesGraph
    {
        /// <inheritdoc/>
        public Dictionary<int, GmshVolume> Volumes { get; private set; }
        /// <inheritdoc/>
        public Dictionary<int, GmshSurface> Surfaces { get; private set; }
        /// <inheritdoc/>
        public Dictionary<int, GmshCurve> Curves { get; private set; }
        /// <inheritdoc/>
        public Dictionary<int, GmshPoint> Points { get; private set; }
        /// <summary>
        /// Строит граф смежности для текущей модели, в модели должны присутствовать объемы иначе граф не построится
        /// </summary>
        /// <param name="controller"></param>
        public GmshEntitiesGraph(IGmshController controller)
        {
            int[] dimTags;
            controller.ModelGetGeometryEntities(out dimTags, 3);
            for (var i = 0; i < dimTags.Length; i += 2)
            {
                var volumeTag = dimTags[i + 1];
                var surfTags = GetVolumeAdjacencies(controller, volumeTag);
                for (var j = 0; j < surfTags.Length; ++j)
                {
                    var surfaceTag = surfTags[j];
                    var curveTags = GetSurfaceAdjacencies(controller, Volumes[volumeTag], surfaceTag);
                    for(var k = 0; k < curveTags.Length; ++k)
                    {
                        var curveTag = curveTags[k];
                        var pointTags = GetCurveAdjacencies(controller, Surfaces[surfaceTag], curveTag);
                        for(var l = 0; l < pointTags.Length; ++l)
                        {
                            ProcessPoints(controller, Curves[curveTag], pointTags[l]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Вернуть нижних соседей текущего объема
        /// </summary>
        /// <param name="controller">Контроллер Gmsh</param>
        /// <param name="volumeTag">Идентификатор объема</param>
        /// <returns>Идентифкаторы поверхностей текущего объема</returns>
        private int[] GetVolumeAdjacencies(IGmshController controller, int volumeTag)
        {
            if (Volumes == null)
                Volumes = new Dictionary<int, GmshVolume>();
            var volume = new GmshVolume(volumeTag);
            Volumes.Add(volumeTag, volume);
            int[] temp, surfTags;
            controller.ModelGetAdjacencies(3, volumeTag, out temp, out surfTags);
            return surfTags;
        }

        /// <summary>
        /// Вернуть нижних соседей текущей поверхности
        /// </summary>
        /// <param name="controller">Контроллер Gmsh</param>
        /// <param name="volume">Текущий объем</param>
        /// <param name="surfaceTag">Идентификатор поверхности</param>
        /// <returns>Идентификаторы кривых</returns>
        private int[] GetSurfaceAdjacencies(IGmshController controller, GmshVolume volume, int surfaceTag)
        {
            if (Surfaces == null)
                Surfaces = new Dictionary<int, GmshSurface>();
            if(!Surfaces.ContainsKey(surfaceTag))
                Surfaces.Add(surfaceTag, new GmshSurface(surfaceTag));
            var surface = Surfaces[surfaceTag];
            volume.Surfaces.Add(surfaceTag, surface);
            surface.Volumes.Add(volume.Tag, volume);
            int[] temp, curveTags;
            controller.ModelGetAdjacencies(2, surfaceTag, out temp, out curveTags);
            return curveTags;
        }
        /// <summary>
        /// Вернуть нижних соседей текущей кривой
        /// </summary>
        /// <param name="controller">Контроллер Gmsh</param>
        /// <param name="surface">Текущая поверхность</param>
        /// <param name="curveTag">Идентификатор кривой</param>
        /// <returns>Идентифкаторы геометрических точек</returns>
        private int[] GetCurveAdjacencies(IGmshController controller, GmshSurface surface, int curveTag)
        {
            if (Curves == null)
                Curves = new Dictionary<int, GmshCurve>();
            if (!Curves.ContainsKey(curveTag))
                Curves.Add(curveTag, new GmshCurve(curveTag));
            var curve = Curves[curveTag];
            surface.Curves.Add(curveTag, curve);
            curve.Surfaces.Add(surface.Tag, surface);
            int[] temp, pointTags;
            controller.ModelGetAdjacencies(1, curveTag, out temp, out pointTags);
            return pointTags;
        }
        /// <summary>
        /// Обработать последний уровень точек, связать соседей между собой
        /// </summary>
        /// <param name="controller">Контроллер Gmsh</param>
        /// <param name="curve">Текущая кривая</param>
        /// <param name="pointTag">Текущая геометрическая точка</param>
        private void ProcessPoints(IGmshController controller, GmshCurve curve, int pointTag)
        {
            if (Points == null)
                Points = new Dictionary<int, GmshPoint>();
            if (!Points.ContainsKey(pointTag))
                Points.Add(pointTag, new GmshPoint(pointTag));
            var point = Points[pointTag];
            curve.Points.Add(pointTag, point);
            point.Curves.Add(curve.Tag, curve);
        }
    }
}
