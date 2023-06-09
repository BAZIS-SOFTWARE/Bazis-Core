using Geometry;
using Model;
using ModelParcer.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Класс представляет данные модели
    /// </summary>
    public class GmshUserModelData
    {
        protected List<Node> nodes;
        protected List<Curve> curves;
        protected List<Surface> surfaces;
        /// <summary>
        /// Получает идентификатор с ассоциированной моделью
        /// </summary>
        protected long Id { get; private set; }
        /// <summary>
        /// Экземпляр данных модели
        /// </summary>
        /// <param name="id">[In]Идентификатор модели</param>
        public GmshUserModelData(long id)
        {
            Id = id;
            var error = 0;
            GmshMesh.gmshModelSetCurrent(Id.ToString(), ref error);
        }
        /// <summary>
        /// Получает данные о модели
        /// </summary>
        protected virtual void GetElements()
        {
        }

        protected void GetPoints()
        {
            var error = 0;
            IntPtr nodeTags, nodeTags_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTags = nodeTags_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref coord, ref coord_n,
                                             ref paramCoord, ref paramCoord_n, -1, -1, 0, 0, ref error);
            long[] tagPoints;
            double[] coords;
            var pointsCount = nodeTags_n.ToInt32();
            nodes = new List<Node>(pointsCount);
            GmshUserMemory.GetTypesData(nodeTags, out tagPoints, nodeTags_n);
            GmshUserMemory.GetTypesData(coord, out coords, coord_n);
            /*GetTypesData(out tagPoints, nodeTags, nodeTags_n);
            GetTypesData(out coords, coord, coord_n);*/
            for (var i = 0; i < pointsCount; ++i)
            {
                var stride = i * 3;
                nodes.Add(new Node((int)tagPoints[i], new Point3D((float)coords[stride], (float)coords[stride + 1],
                                                                 (float)coords[stride + 2])));
            }
        }
    }
}
