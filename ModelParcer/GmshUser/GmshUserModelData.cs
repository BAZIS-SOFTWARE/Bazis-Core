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
        /// <summary>
        /// Класс точка 3D двойной точности
        /// </summary>
        protected class DoublePoint3D
        {
            private int number;
            private double x;
            private double y;
            private double z;

            public double[] ToArray { get { return new double[] { x, y, z }; } }

            public Node ToNode { get { return new Node(number, new Point3D((float)x, (float)y, (float)z));} }

            public DoublePoint3D(int number, double x, double y, double z)
            {
                this.number = number;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        protected List<DoublePoint3D> nodes;
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
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
        }
        /// <summary>
        /// Получает данные о модели
        /// </summary>
        protected virtual void GetElements()
        {
        }

        /// <summary>
        /// Получает точки сетки
        /// </summary>
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
            nodes = new List<DoublePoint3D>(pointsCount);
            GmshUserMemory.GetTypesData(nodeTags, out tagPoints, nodeTags_n);
            GmshUserMemory.GetTypesData(coord, out coords, coord_n);
            for (var i = 0; i < pointsCount; ++i)
            {
                var stride = i * 3;
                nodes.Add(new DoublePoint3D((int)tagPoints[i],coords [stride], coords[stride + 1],
                                            coords[stride + 2]));
            }
        }
    }
}
