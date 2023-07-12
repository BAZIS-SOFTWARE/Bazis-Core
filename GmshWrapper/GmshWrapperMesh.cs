using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Model.GmshController;
using Model.GmshUser;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Класс содержит результат извлечения типов элементов
    /// </summary>
    public class ElementInfo
    {
        /// <summary>
        /// Имя элемента
        /// </summary>
        public string elementName;
        /// <summary>
        /// Размерность
        /// </summary>
        public int dim;
        /// <summary>
        /// Порядок
        /// </summary>
        public int order;
        /// <summary>
        /// Количество узлов
        /// </summary>
        public int numNodes;
        /// <summary>
        /// Массив координат
        /// </summary>
        public double[] coords;
        /// <summary>
        /// Количество координат
        /// </summary>
        public int coords_n;
        /// <summary>
        /// Главные узлы
        /// </summary>
        public int primNodes;
    }
    /// <summary>
    /// Оборачивает основные методы сетки полученные из GmshController
    /// </summary>
    public static class GmshWrapperMesh
    {
        /// <summary>
        /// [In]Размерность в которой нужно сгенерировать модель
        /// </summary>
        /// <param name="dim"></param>
        public static void Generate(int dim)
        {
            var ierr = 0;
            if (GmshWrapperGeometry.GetDimension() == dim)
                GmshMesh.gmshModelMeshGenerate(-1, ref ierr);
            GmshMesh.gmshModelMeshGenerate(dim, ref ierr);
        }
        /// <summary>
        /// Переводит большую часть сетки в четырехугольники
        /// </summary>
        public static void Recombine()
        {
            var ierr = 0;
            GmshMesh.gmshModelMeshRecombine(ref ierr);
        }
        /// <summary>
        /// Равномерно уплотняет сетку у текущей модели
        /// </summary>
        public static void Refine()
        {
            var ierr = 0;
            GmshMesh.gmshModelMeshRefine(ref ierr);
        }
        /// <summary>
        /// Получает свойство элемента
        /// </summary>
        /// <param name="elementTypes">[In]Тип элемента</param>
        /// <returns>Массив ElementInfo</returns>
        public static ElementInfo[] GetElementProperties(int[] elementTypes)
        {
            var ierr = 0;
            IntPtr localeNodeCoord, localeNodeCoord_n, elementName;
            localeNodeCoord = localeNodeCoord_n = elementName = IntPtr.Zero;
            var dataInfo = new ElementInfo[elementTypes.Length];
            for (var i = 0; i < elementTypes.Length; ++i)
            {
                var info = new ElementInfo();
                GmshMesh.gmshModelMeshGetElementProperties(elementTypes[i], ref elementName, ref info.dim, ref info.order,
                                                           ref info.numNodes, ref localeNodeCoord, ref localeNodeCoord_n,
                                                           ref info.primNodes, ref ierr);
                info.elementName = GmshWrapperMemory.GetName(elementName);
                GmshUserMemory.GetTypesData(localeNodeCoord, out info.coords, localeNodeCoord_n);
                info.coords_n = info.coords.Length;
                dataInfo[i] = info;
            }
            return dataInfo;
        }
        /// <summary>
        /// Получает все элементы в текущей модели, их идентификаторы и идентификаторы точек внутри элемента
        /// </summary>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Индекс элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="elemInfo">[Out]Массив ElementInfo для заполнения</param>
        /// <param name="elemsTags">[Out]Массив elementTags для заполнения</param>
        /// <param name="nodesTags">[Out]Массив elementTags для заполнения</param>
        public static void GetElements(int dim, int tag, out ElementInfo[] elemInfo, 
                                       out int[][] elemsTags, out int[][] nodesTags)
        {
            var ierr = 0;
            IntPtr eTypes, eTypes_n, eTags, eTags_n, eTags_nn, nodeTags, nodeTags_n, nodeTags_nn;
            eTypes = eTypes_n = eTags = eTags_n = eTags_nn = nodeTags = nodeTags_n = nodeTags_nn = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetElements(ref eTypes, ref eTypes_n,
                                              ref eTags, ref eTags_n, ref eTags_nn,
                                              ref nodeTags, ref nodeTags_n, ref nodeTags_nn, dim, tag, ref ierr);
            var elemTypes = GmshWrapperMemory.GetTypesData<int>(eTypes, eTypes_n);
            elemInfo = GetElementProperties(elemTypes);
            elemsTags = GmshWrapperMemory.GetElementData<int>(eTags, eTags_n, eTags_nn);
            nodesTags = GmshWrapperMemory.GetElementData<int>(nodeTags, nodeTags_n, nodeTags_nn);
        }
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
            return GmshWrapperMemory.GetTypesData<int>(elementTags, elementTags_n);
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
