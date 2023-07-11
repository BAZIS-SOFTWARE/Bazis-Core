using System;
using Model.GmshController;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Оборачивает основные методы сетки полученные из GmshController
    /// </summary>
    public static class GmshWrapperMesh
    {
        /// <summary>
        /// Находит элементы сетки по переданной координате x,y,z
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="strict">[In]Выполнять строгий поиск?</param>
        public static int[] GetElementsByCoordinates(int dim, double x, double y, double z, bool strict = false)
        {
            var ierr = 0;
            var strictInt = Convert.ToInt32(strict);
            IntPtr elementTags, elementTags_n;
            elementTags = elementTags_n = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetElementsByCoordinates(x, y, z, ref elementTags, ref elementTags_n,
                                                           dim, strictInt, ref ierr);
            int[] elemTags;
            GmshWrapperMemory.GetTypesData(elementTags, out elemTags, elementTags_n);
            return elemTags;
        }
        /// <summary>
        /// Возвращает локальные координаты элемента с заданным идентификатором
        /// </summary>
        /// <param name="elementTag">[In]Идентификатор элемента</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <returns>Массив локальных координат uvw</returns>
        public static double[] GetLocalCoordinatesInElement(uint elementTag, double x, double y, double z)
        {
            var ierr = 0;
            var elTag = new UIntPtr(elementTag);
            var result = new double[3];
            GmshMesh.gmshModelMeshGetLocalCoordinatesInElement(elTag, x, y, z, ref result[0], ref result[1],
                                                               ref result[2]);
            return result;
        }
    }
}
