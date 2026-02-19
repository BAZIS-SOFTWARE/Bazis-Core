using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс трансфинитной экструзии 3д элементов (гекса и пента элементы) методом вращения
    /// </summary>
    public class TransfiniteRotateExtruder : RotateExtruder
    {
        /// <summary>
        /// Создает экструзию 3д элементов методом поворота вдоль указанного локального вектора и точке начала координат 
        /// </summary>
        /// <param name="curLayer">Текущий слой для экструзии</param>
        /// <param name="origin">Точка начала координат</param>
        /// <param name="rotAxis">Локальная ось поворота</param>
        /// <param name="segments">Число сегментов</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException">Число сегментов не должно быть меньше 3</exception>
        public override List<List<Element3D>> Extrude(IEnumerable<Element2D> curLayer, Vector3 origin, Vector3 rotAxis, int segments = 8)
        {
            if (segments < 3)
                throw new ArgumentException("Количество сегментов не должно быть меньше 3");

            rotAxis = Vector3.Normalize(rotAxis);
            var tetLayers = new List<List<Element3D>>();
            curLayer = SortFirstLayer(curLayer);
            var firstLayer = CreateFirstSurface(curLayer);
            curLayer = firstLayer;

            var offset = 2 * MathF.PI / segments;
            var rotMatrix = Matrix4x4.CreateTranslation(origin)
                              * Matrix4x4.CreateFromAxisAngle(rotAxis, offset)
                              * Matrix4x4.CreateTranslation(-origin);
            rotMatrix = Matrix4x4.Transpose(rotMatrix);

            List<Element2D> nextLayer = null;
            for (var i = 0; i < segments - 1; ++i)
            {
                nextLayer = CreateNextLayer(curLayer, rotMatrix);
                var layer = CreateTransfiniteLayer(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            var lastLayer = CreateTransfiniteLayer(nextLayer, firstLayer);
            tetLayers.Add(lastLayer);
            return tetLayers;
        }
    }
}
