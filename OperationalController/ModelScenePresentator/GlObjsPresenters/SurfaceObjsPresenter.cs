using Geometry.Interfaces;
using Model.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы поверхностных объектов с готовыми цветами.
    /// </summary>
    public class SurfaceObjsPresenter<T, U> : ObjsPresenter<ISurfaceObject<T, U>>, ISurfaceObjsPresenter
        where T : ISurface<U>
        where U : IPoint
    {
        private readonly bool drawInnerSurfaces;

        /// <summary>
        /// Создаёт presenter поверхностных объектов с индивидуальными цветами.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors, bool drawInnerSurfaces = true)
            : this(surfaceObjects, new ManyColorsObjsColors(colors), drawInnerSurfaces)
        {
        }

        /// <summary>
        /// Создаёт presenter поверхностных объектов с одним цветом.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color, bool drawInnerSurfaces = true)
            : this(surfaceObjects, new SingleColorObjsColors(color), drawInnerSurfaces)
        {
        }

        /// <summary>
        /// Создаёт presenter поверхностных объектов с заданным источником цветов.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IObjsColors colors, bool drawInnerSurfaces = true)
            : base(surfaceObjects, colors)
        {
            this.drawInnerSurfaces = drawInnerSurfaces;
        }

        /// <inheritdoc/>
        public override PresenterType PresenterType { get; } = PresenterType.Surface;

        /// <inheritdoc/>
        public override ViewMode ViewMode { get; set; } = ViewMode.LineSurface;

        /// <inheritdoc/>
        public override Tuple<int, int, int, int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var edges = 0;
            var colorsCount = 0;

            for (var index = 0; index < Objects.Count; index++)
            {
                foreach (var surface in GetDrawnSurfaces(index))
                {
                    var triangles = surface.NumberOfPoints - 2;
                    vertexes += triangles * 9;
                    pointers += triangles * 3;
                    edges += triangles * 3;
                    colorsCount += triangles * 12;
                }
            }

            return new Tuple<int, int, int, int>(pointers, vertexes, colorsCount, edges);
        }

        /// <inheritdoc/>
        public override float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexes = new float[vertexLength];
            var stride = 0;
            ISurfaceObjsVertexCreator<T, U> vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorSurfaceObjsCreator<T, U>();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsSurfaceObjsCreator<T, U>();
            else
                vertexCreator = new NormalsSurfaceObjsCreator<T, U>();

            var objectIndex = 0;

            foreach (var color in CreateColors())
            {
                if (objectIndex == Objects.Count)
                    throw new InvalidOperationException("More colors were provided than objects.");

                vertexCreator.Create(ref stride, vertexes, GetDrawnSurfaces(objectIndex), color);
                objectIndex++;
            }

            if (objectIndex != Objects.Count)
                throw new InvalidOperationException("A color was not provided for every object.");

            return vertexes;
        }

        /// <inheritdoc/>
        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            var edges = new bool[edgesLength];
            var stride = 0;
            var creator = new EdgesSurfaceObjsCreator<T, U>();

            for (var index = 0; index < Objects.Count; index++)
                creator.Create(ref stride, edges, GetDrawnSurfaces(index));

            return edges;
        }

        /// <inheritdoc/>
        public int[] CreateSeparators()
        {
            var separators = new int[Objects.Count + 1];
            var stride = 0;

            for (var index = 0; index < Objects.Count; index++)
            {
                separators[index] = stride;

                foreach (var surface in GetDrawnSurfaces(index))
                    stride += surface.NumberOfPoints - 2;
            }

            separators[Objects.Count] = stride;
            return separators;
        }

        /// <summary>
        /// Отбирает грани объекта, которые попадают в буфер.
        /// </summary>
        private IEnumerable<T> GetDrawnSurfaces(int index)
        {
            foreach (var surface in Objects[index].GetSurfaces())
            {
                if (drawInnerSurfaces || !surface.IsInner)
                    yield return surface;
            }
        }
    }
}
