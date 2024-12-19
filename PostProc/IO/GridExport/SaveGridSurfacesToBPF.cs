using Geometry;
using PostProc.ScenePresenter;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Results;
using Results.ResultsData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProc.IO.GridExport
{
    public class SaveGridSurfacesToBPF : IGridSurfacesSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaveEventArgs> SaveEvent;

        /// <inheritdoc/>
        public void SaveGrid(IEnumerable<ResultFigure> figures, string savePath)
        {
            var elements = new Dictionary<int, List<string>>();
            var points = new Dictionary<Point3D, int>();

            var pointsCounter = 0;
            var elementsCounter = figures.Select(x => x.NumberOfPoints).Count() - 1;
            foreach (var f in figures)
            {
                elements.Add(elementsCounter, new List<string>());
                foreach (var cords in f.GetCoordinates())
                {
                    if (!points.Keys.Contains(cords))
                    {
                        points.Add(cords, pointsCounter);
                        elements[elementsCounter].Add(pointsCounter.ToString());
                        pointsCounter++;
                    }
                    else
                        elements[elementsCounter].Add(points[cords].ToString());
                }
                elementsCounter++;
            }

            using (var sw = new StreamWriter(savePath, true, Encoding.Unicode) { AutoFlush = true })
            {


                sw.WriteLine("");
                sw.WriteLine("Узлы");
                foreach (var p in points)
                    sw.WriteLine($"{p.Value} {p.Key.ToString().Replace('|', ' ')}");
                sw.WriteLine("#Узлы");

                sw.WriteLine("Элемент2D");
                foreach (var e in elements)
                    sw.WriteLine($"{e.Key} {string.Join(" ", e.Value)}");
                sw.WriteLine("#Элемент2D");

                sw.WriteLine("Элемент3D");
                sw.WriteLine("#Элемент3D");
            }
        }
    }
}
