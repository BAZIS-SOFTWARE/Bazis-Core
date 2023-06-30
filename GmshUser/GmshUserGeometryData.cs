using PrFunctionLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.GroupsData;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using Model.GmshController;
using System.Diagnostics.Tracing;
using Model.Interfaces;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Model.GmshUser
{
    /// <summary>
    /// Извлекает данные о геометрии заданной модели
    /// </summary>
    public static class GmshUserGeometryData
    {
        private delegate void Derivative(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n,
                                         ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        /// <summary>
        /// Возвращает массив всех пар размерность : идентификатор
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <returns>Массив пар размерность : идентификатор</returns>
        public static int[] GetEntities(long modelId)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            var ext = Path.GetExtension(GmshUserIO.GetFileName());
            IntPtr dimTags, dimTags_n, outDimTags, outDimTags_n;
            dimTags = dimTags_n = outDimTags = outDimTags_n = IntPtr.Zero;
            if (ext.Contains("step") || ext.Contains("igs") || ext.Contains("brep"))
                GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            else
                GmshModel.gmshModelGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            int[] data;
            GmshUserMemory.GetTypesData(dimTags, out data, dimTags_n);
            return data;
        }
        /// <summary>
        /// Возвращает локальные координаты точек по набору параметрических координат или без них
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="parametric">[In]Массив параметрических координат (может быть null)</param>
        /// <param name="dimTag">[In]Пара размерность идентификатор</param>
        /// <returns>Локальные координаты точки</returns>
        public static double[] GetPoints(long modelId, double[] parametric, Tuple<int, int> dimTag)
        {
            var error = 0;
            var length = parametric == null ? IntPtr.Zero : (IntPtr)parametric.Length;
            var coords = IntPtr.Zero;
            var coords_n = IntPtr.Zero;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshModel.gmshModelGetValue(dimTag.Item1, dimTag.Item2, parametric, length, ref coords, ref coords_n, ref error);
            double[] points;
            GmshUserMemory.GetTypesData(coords, out points, coords_n);
            return points;
        }
        /// <summary>
        /// Возвращает границы параметризации для пар размерность : идентификатор
        /// </summary>
        /// <param name="modelId">[In]Идентифкатор модели</param>
        /// <param name="paramsCount">[In]Количество параметров U[V]</param>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <returns>[minU,maxU] или [minU,maxU],[minV,maxV]</returns>
        public static double[][] GetParametrizationBounds(long modelId, int paramsCount, Tuple<int, int> dimTag)
        {
            var error = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshModel.gmshModelGetParametrizationBounds(dimTag.Item1, dimTag.Item2, ref min, ref min_n, 
                                                        ref max, ref max_n, ref error);
            double[] minBounds, maxBounds;
            GmshUserMemory.GetTypesData(min, out minBounds, min_n);
            GmshUserMemory.GetTypesData(max, out maxBounds, max_n);
            var result = new double[paramsCount][];
            for (var i = 0; i < paramsCount; ++i)
            {
                result[i] = new double[2];
                result[i][0] = minBounds[i];
                result[i][1] = maxBounds[i];
            }
            return result;
        }
        /// <summary>
        /// Возвращает частную производную по параметрическим координатам
        /// </summary>
        /// <param name="modelId">[In]Идентифкатор модели</param>
        /// <param name="parametrs">[In]Параметрические координаты</param>
        /// <param name="derNum">[In]Номер производной ["first","second"]</param>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <returns></returns>
        public static double[] GetDerivative(long modelId, double[] parametrs, string derNum, Tuple<int,int> dimTag)
        {
            Derivative derFun = GmshGeometry.gmshModelGetDerivative; 
            if (derNum == "second") 
                derFun = GmshGeometry.gmshModelGetSecondDerivative;
            var error = 0;
            var derivaties = IntPtr.Zero;
            var derivaties_n = IntPtr.Zero;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            derFun(dimTag.Item1, dimTag.Item2, parametrs, (IntPtr)parametrs.Length, 
                   ref derivaties, ref derivaties_n, ref error);
            double[] derivative;
            GmshUserMemory.GetTypesData(derivaties, out derivative, derivaties_n);
            return derivative;
        }
        /// <summary>
        /// Является ли кривая линией
        /// </summary>
        /// <param name="modelId">[In]Идентифкатор модели</param>
        /// <param name="paramsBound">[In]Параметрические координаты</param>
        /// <param name="dimTag">[In]Пара рамзерность : идентифкатор</param>
        /// <returns>Является ли кривая линией</returns>
        public static bool IsLine(long modelId, double[] paramsBound, Tuple<int, int> dimTag)
        {
            var modParams = new double[] { paramsBound[0] + 0.01, paramsBound[1] - 0.01 };
            var derivatives = GetDerivative(modelId, paramsBound, "first", dimTag);
            var derCount = derivatives.Length / 3;
            for (var i = 1; i < derCount; ++i)
                for (var j = 0; j < 3; ++j)
                    if (Math.Abs(derivatives[i * 3 + j] - derivatives[(i - 1) * 3 + j]) > 1E-8)
                        return false;
            return true;
        }
        /// <summary>
        /// Возвращает максимальную кривизну кривой/поверхности по параметрическим кординатам
        /// </summary>
        /// <param name="modelId">[In]Идентифкатор модели</param>
        /// <param name="parametrs">[In]</param>
        /// <param name="dimTag">[In]Пара рамзерность : идентифкатор</param>
        /// <returns>Массив кривизны</returns>
        public static double[] GetCurvatures(long modelId, double[] parametrs, Tuple<int, int> dimTag)
        {
            var error = 0;
            var curvatures = IntPtr.Zero;
            var curvatures_n = IntPtr.Zero;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshGeometry.gmshModelGetCurvature(dimTag.Item1, dimTag.Item2, parametrs, (IntPtr)parametrs.Length,
                                               ref curvatures, ref curvatures_n, ref error);
            double[] curvature;
            GmshUserMemory.GetTypesData(curvatures, out curvature, curvatures_n);
            return curvature;
        }
        /// <summary>
        /// Задает минимальную плотность точек
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="min">[In]Минимальное значение</param>
        public static void SetMinDencity(long modelId, double min)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
        }
        /// <summary>
        /// Получает минимальную плотность точек
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <returns></returns>
        public static double GetMinDencity(long modelId)
        {
            var error = 0;
            double min = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMin", ref min, ref error);
            return min;
        }
        /// <summary>
        /// Получает максимальную плотность точек
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="max">[In]Максимальное значение</param>
        public static void SetMaxDencity(long modelId, double max)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
        }
        /// <summary>
        /// Получает максимальную плотность точек
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <returns></returns>
        public static double GetMaxDencity(long modelId)
        {
            var error = 0;
            double max = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMax", ref max, ref error);
            return max;
        }
    }
}
