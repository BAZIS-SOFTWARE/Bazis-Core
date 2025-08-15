using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PostProc.ScenePresenter;

namespace PostProc.IO.GridExport
{
    public abstract class SaveGridSurfacesToSTLBase : IGridSurfacesSaver
    {
        public virtual event Action<object, string> SaveEvent;

        /// <summary>
        /// триангуляция выпуклого односвязного многоугольника
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        protected IEnumerable<List<Point3D>> SplitToTriangles(ResultSurface surface)
        {
            var points = surface.GetCoordinates().ToList();

            for (int i = 0; i < points.Count - 2; i++)
            {
                yield return new List<Point3D>()
                {
                    points[0],
                    points[i + 1],
                    points[i + 2]
                };
            }
        }

        public abstract void SaveGrid(IEnumerable<ResultFigure> figures, string savePath);
    }
}