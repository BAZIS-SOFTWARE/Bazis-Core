using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.GmshController;

namespace Model.GmshUser
{
    /// <summary>
    /// Извлекает данные о заданной модели
    /// </summary>
    public static class GmshModelObjectData
    {
        /// <summary>
        /// Возвращает ограничивающий параллелипипед для пары размерность : идентификатор
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Tuple[Point minCoords,Point maxCoords]</returns>
        public static Tuple<Point3D, Point3D> GetBoundingBox(long modelId,int dim, int tag)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            var minBounds = new double[3];
            var maxBounds = new double[3];
            GmshGeometry.gmshModelGetBoundingBox(dim, tag, ref minBounds[0], ref minBounds[1], ref minBounds[2],
                                              ref maxBounds[0], ref maxBounds[1], ref maxBounds[2], ref error);
            return Tuple.Create(new Point3D((float)minBounds[0], (float)minBounds[1], (float)minBounds[2]),
                                new Point3D((float)maxBounds[0], (float)maxBounds[1], (float)maxBounds[2]));
        }
        /// <summary>
        /// Возвращает весь список имен моделей
        /// </summary>
        /// <returns>Список имен моделей</returns>
        public static string[] ModelList()
        {
            var error = 0;
            IntPtr names = IntPtr.Zero;
            IntPtr names_n = IntPtr.Zero;
            GmshModel.gmshModelList(ref names, ref names_n, ref error);
            string[][] data;
            return GmshUserMemory.GetArrayOfNames(names, names_n);
        }
        /// <summary>
        /// Возвращает матрицу смежности для текущей пары dim : tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>[i][]Нижние элементы,[i + 1][]Верхние элементы</returns>
        public static int[][] GetAdjacencies(int dim, int tag)
        {
            var error = 0;
            IntPtr upward, upward_n, downward, downward_n;
            upward = upward_n = downward = downward_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetAdjacencies(dim, tag, ref upward, ref upward_n, 
                                              ref downward, ref downward_n, ref error);
            int[] outDownward, outUpward;
            GmshUserMemory.GetTypesData(downward, out outDownward, downward_n);
            GmshUserMemory.GetTypesData(upward, out outUpward, upward_n);
            return new int[2][] { outDownward, outUpward };
        }
    }
}
