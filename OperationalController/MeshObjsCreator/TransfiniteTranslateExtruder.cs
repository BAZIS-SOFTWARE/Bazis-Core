using Geometry;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс трансфинитной экструзии 3д элементов (гекса и пента элементы) вдоль вектора
    /// </summary>
    public class TransfiniteTranslateExtruder : TranslateExtruder
    {
        /// <summary>
        /// Создает трансфинитную экструзию 3д элементов по указанному слою 2д элементов
        /// </summary>
        /// <param name="curLayer">Текущий слой 2д элементов</param>
        /// <param name="dir">Вектор направления экструзии</param>
        /// <param name="layers">Количество слоев</param>
        /// <param name="layerSize">Высота слоя</param>
        /// <returns>Список 3д элементов</returns>
        public override List<List<Element3D>> Extrude(IEnumerable<Element2D> curLayer, Point3D dir, int layers = 1, float layerSize = 1)
        {
            dir = Vector.GetVectorNorm(dir);
            var layerDir = dir.Mult(layerSize);

            var tetLayers = new List<List<Element3D>>();

            var sortedLayer = SortFirstLayer(curLayer);
            curLayer = CreateFirstSurface(sortedLayer);

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextLayer(curLayer, layerDir);
                var layer = CreateTransfiniteLayer(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            return tetLayers;
        }
    }
}
