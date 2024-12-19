using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using PostProc.ScenePresenter;
using Project.IO;

namespace PostProc.IO.GridExport
{

    /// <summary>
    /// Экспорт результатов в формат бинарного STL
    /// </summary>
    public class SaveGridSurfacesToBinSTL : SaveGridSurfacesToSTLBase
    {
        public virtual event Action<object, ISaveEventArgs> SaveEvent;
        public override void SaveGrid(IEnumerable<ResultFigure> figures, string savePath)
        {
            ushort attrib = 0;

            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.OpenOrCreate)))
            {
                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                byte[] buffer = Encoding.UTF8.GetBytes("Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}");
                var captionBytes = new byte[Math.Max(buffer.Length, 80)];
                Array.Copy(buffer, captionBytes, Math.Min(buffer.Length, 80));
                writer.Write(captionBytes); // – Header
                var count = figures.Select(w => w.GetSurfaces().Count()).Sum();

                writer.Write((uint)count); // – Number of triangles

                foreach (var item in figures)
                {
                    foreach (var surface in item.GetSurfaces())
                    {
                        var triangles = SplitToTriangles(surface);
                        foreach (var points in triangles)
                        {
                            Point3D norm = surface.CalcNormal();
                            writer.Write(norm._x);
                            writer.Write(norm._y);
                            writer.Write(norm._z);

                            foreach (var point in points) // Vertex 1,2,3
                            {
                                writer.Write(point._x);
                                writer.Write(point._y);
                                writer.Write(point._z);
                            }
                            writer.Write(attrib); // – Attribute byte count
                        }
                    }

                    unchecked
                    {
                        attrib++;
                    }

                }
            }

            SaveEvent?.Invoke(this, new SaveEventArgs($"Данные сохранены в файле {savePath}"));
        }
    }
}
