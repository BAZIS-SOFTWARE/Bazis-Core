using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Geometry;
using PostProc.ScenePresenter;
using Project.Interfaces.IO;
using Project.Interfaces.Results;
using Project.IO;

namespace PostProc.IO.GridExport
{
    /// <summary>
    /// Экспорт результатов в формат текстого STL
    /// </summary>
    public class SaveGridSurfacesToTxtSTL : SaveGridSurfacesToSTLBase
    {
        /// <inheritdoc/>
        public virtual event Action<object, ISaveEventArgs> SaveEvent;

        /// <inheritdoc/>
        public override void SaveGrid(IEnumerable<ResultFigure> figures, string savePath)
        {
            using (var writer = new StreamWriter(savePath))
            {
                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;

                foreach (var item in figures)
                {
                    writer.WriteLine("solid Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}"); // – Header
                    foreach (var surface in item.GetSurfaces())
                    {
                        var triangles = SplitToTriangles(surface);
                        foreach (var points in triangles)
                        {
                            Point3D norm = surface.CalcNormal();
                            writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "   facet normal {0:R} {1:R} {2:R}",
                                norm._x, norm._y, norm._z));
                            writer.WriteLine($"      outer loop");

                            foreach (var point in points) // Vertex 1,2,3
                            {
                                writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "         vertex {0:R} {1:R} {2:R}",
                                    point._x, point._y, point._z));
                            }
                            writer.WriteLine($"      endloop");
                            writer.WriteLine($"   endfacet");
                        }


                    }
                    writer.WriteLine($"endsolid");
                }

            }

            SaveEvent?.Invoke(this, new SaveEventArgs($"Данные сохранены в файле {savePath}"));
        }
    }
}