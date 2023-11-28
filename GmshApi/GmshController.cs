using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Geometry;
using Model;
using ModelInterfaces;
using Functions.Extensions;
using System.IO;
using Model.Elements;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Класс содержит информацию о типе извлечнного элемента
    /// </summary>
    public class ElementProperties
    {
        /// <summary>Имя элемента</summary>
        public string elementName;
        /// <summary>Размерность</summary>
        public int dim;
        /// <summary>Порядок</summary>
        public int order;
        /// <summary>Количество узлов</summary>
        public int numNodes;
        /// <summary>Массив координат</summary>
        public double[] coords;
        /// <summary>Количество координат</summary>
        public int coords_n;
        /// <summary>Главные узлы</summary>
        public int numPrimaryNodes;
    }
    /// <summary>
    /// Контроллер GmshApi
    /// </summary>
    public class GmshController
    {
        /// <summary>
        /// Указатель на DLL
        /// </summary>
        public IntPtr DLL { get; private set; }

        private string GetStringPtr(IntPtr name)
        {
            return Marshal.PtrToStringAnsi(name);
        }

        string[] geomExtensions = new string[]
        {
            "step","stp","brep","iges","igs","stl"
        };
        #region "Initialization Delegates"
        /// <summary>
        /// Инициализирует Gmsh Api
        /// </summary>
        /// <param name="argc">[In]Количество строковых аргументов командной строки</param>
        /// <param name="argv">[In]Массив строк аргуметов командной строки</param>
        /// <param name="readConfigFile">[In]Флаг - читать конфигурационный файл gmsh</param>
        /// <param name="run">[In]Флаг запуска приложения в командной строке</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshInitializeFun(int argc, ref string argv, int readConfigFile, int run, ref int ierr);///
        public gmshInitializeFun gmshInitialize;
        /// <summary>
        /// Возвращает флаг инициализировано ли API
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Инициализировано ли API</returns>
        public delegate int gmshIsInitializedFun(ref int ierr);///
        public gmshIsInitializedFun gmshIsInitialized;
        /// <summary>
        /// Завершает работу Gmsh Api
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshFinalizeFun(ref int ierr);///
        public gmshFinalizeFun gmshFinalize;
        #endregion
        #region "Memory Delegates"
        /// <summary>
        /// Выделяет неуправляемую память
        /// </summary>
        /// <param name="n">[In]Количество байт</param>
        /// <returns>Указатель на неуправляемую память</returns>
        private delegate IntPtr gmshMallocFun(IntPtr n);
        private gmshMallocFun gmshMalloc;
        /// <summary>
        /// Очищает неуправляемую память
        /// </summary>
        /// <param name="p">[In]Указатель на блок памяти</param>
        private delegate void gmshFreeFun(IntPtr p);
        private gmshFreeFun gmshFree;
        #endregion
        #region "General Delegates"
        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshOptionSetNumberFun(string name, double value, ref int ierr);///
        public gmshOptionSetNumberFun gmshOptionSetNumber;
        /// <summary>
        /// Возвращает значение из поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[Out]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshOptionGetNumberFun(string name, ref double value, ref int ierr);///
        public gmshOptionGetNumberFun gmshOptionGetNumber;
        /// <summary>
        /// Устанавливает строку параметров для категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshOptionSetStringFun(string name, string value, ref int ierr);///
        public gmshOptionSetStringFun gmshOptionSetString;
        /// <summary>
        /// Возвращает строку параметров для категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[Out]Значения в формате строки для заданной категории опций</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshOptionGetStringFun(string name, ref IntPtr value, ref int ierr);
        private gmshOptionGetStringFun gmshOptionGetString;
        #endregion
        #region "Plugin Delegates"
        /// <summary>
        /// Установить значение через плагин
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[In]Значение в опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshPluginSetNumberFun(string name, string option, double value, ref int ierr);///
        public gmshPluginSetNumberFun gmshPluginSetNumber;
        /// <summary>
        /// Установить набор значений через плагин
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[In]Значения в опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshPluginSetStringFun(string name, string option, string value, ref int ierr);///
        public gmshPluginSetStringFun gmshPluginSetString;
        /// <summary>
        /// Запускает плагин с имененм name и возвращает View если имеется
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Возращает объект View(по необходимости)</returns>
        public delegate int gmshPluginRunFun(string name, ref int ierr);///
        public gmshPluginRunFun gmshPluginRun;
        #endregion
        #region "IO Delegates"
        /// <summary>
        /// Открывает скрипт файл .geo, .stp, .igs создает модель и делает ее текущей
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshOpenFun(string fileName, ref int ierr);///
        public gmshOpenFun gmshOpen;
        /// <summary>
        /// Открывает скрипт и ассоциирует данные с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshMergeFun(string fileName, ref int ierr);///
        public gmshMergeFun gmshMerge;
        /// <summary>
        /// Экспортирует данные модели в файл, расширение задает выходной формат файла
        /// </summary>
        /// <param name="fileName">[In]Путь к файлу назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshWriteFun(string fileName, ref int ierr);///
        public gmshWriteFun gmshWrite;
        #endregion
        #region "Model Delegates"
        /// <summary>
        /// Ассоциирует имя файла с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelSetFileNameFun(string fileName, ref int ierr);///
        public gmshModelSetFileNameFun gmshModelSetFileName;
        /// <summary>
        /// Добавляет новую модель с заданным именем в список моделей и делает ее текущей
        /// </summary>
        /// <param name="name">[In]Имя новой модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelAddFun(string name, ref int ierr);///
        public gmshModelAddFun gmshModelAdd;
        /// <summary>
        /// Удаляет текущую модель из списка моделей
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelRemoveFun(ref int ierr);///
        public gmshModelRemoveFun gmshModelRemove;
        /// <summary>
        /// Очищает список моделей
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshClearFun(ref int ierr);///
        public gmshClearFun gmshClear;
        /// <summary>
        /// Устанавливает имя для текущей модели
        /// </summary>
        /// <param name="name">[In]Имя для текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelSetCurrentFun(string name, ref int ierr);///
        public gmshModelSetCurrentFun gmshModelSetCurrent;
        /// <summary>
        /// Проводит синхронизацию с ядром OpenCascade (например после добавления геометрических сущностей)
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelOccSynchronizeFun(ref int ierr);///
        public gmshModelOccSynchronizeFun gmshModelOccSynchronize;
        /// <summary>
        /// Проводит синхронизацию с ядром gmsh (например после добавления геометрических сущностей)
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelGeoSynchronizeFun(ref int ierr);///
        public gmshModelGeoSynchronizeFun gmshModelGeoSynchronize;
        /// <summary>
        /// Возвращает путь до файла ассоциированный с данной моделью
        /// </summary>
        /// <param name="fileName">[Out]Путь до файла</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetFileNameFun(ref IntPtr fileName, ref int ierr);
        private gmshModelGetFileNameFun gmshModelGetFileName;
        /// <summary>
        /// Возвращает список моделей в gmsh API
        /// </summary>
        /// <param name="names">[Out]Массив имен моделей</param>
        /// <param name="names_n">[Out]Длина массива</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelListFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelListFun gmshModelList;
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <param name="name">[Out]Имя текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetCurrentFun(ref IntPtr name, ref int ierr);
        private gmshModelGetCurrentFun gmshModelGetCurrent;
        /// <summary>
        /// Импортирует геометрию в файл в виде вектора пар размерность : идентификатор
        /// </summary>
        /// <param name="fileName">[In]Файл скрипта</param>
        /// <param name="outDimTags">[Out]Вектор пар размерность : идентификатор</param>
        /// <param name="outDimTags_n">[Out]Длина вектора пар</param>
        /// <param name="highestDimOnly">[In]Импортировать только верхнюю размерность</param>
        /// <param name="format">[In]Опциональная строка с расширением {brep,stp,iges}</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelOccImportShapesFun(string fileName, ref IntPtr outDimTags,
                                                      ref IntPtr outDimTags_n, int highestDimOnly,
                                                      string format, ref int ierr);
        private gmshModelOccImportShapesFun gmshModelOccImportShapes;
        #endregion
        #region "Geometry Delegates"
        /// <summary>
        /// Возвращает список всех идентификаторов для заданной размерности (gmsh)
        /// </summary>
        /// <param name="dimTags">[Out]Массив пар размерность - идентификатор</param>
        /// <param name="dimTags_n">[Out]Длина массива пар размерность - идентификатор</param>
        /// <param name="dim">[In]Размерность для получения идентификаторов, -1 для всех размерностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelGetEntitiesFun gmshModelGetEntities;
        /// <summary>
        /// Возвращает список всех идентификаторов для заданной размерности (OpenCascade)
        /// </summary>
        /// <param name="dimTags">[Out]Массив пар размерность - идентификатор</param>
        /// <param name="dimTags_n">[Out]Длина массива пар размерность - идентификатор</param>
        /// <param name="dim">[In]Размерность для получения идентификаторов, -1 для всех размерностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelOccGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelOccGetEntitiesFun gmshModelOccGetEntities;
        /// <summary>
        /// Удаляет gmsh геометрически сущности заданные парой dimTags
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="recursive">[In]Рекурсивно улалять граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelRemoveEntitiesFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);///
        public gmshModelRemoveEntitiesFun gmshModelRemoveEntities;
        /// <summary>
        /// Удаляет OpenCascade геометрически сущности заданные парой dimTags
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="recursive">[In]Рекурсивно улалять граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelOccRemoveFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);///
        public gmshModelOccRemoveFun gmshModelOccRemove;
        /// <summary>
        /// Задает имя сущности для текущих dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelSetEntityNameFun(int dim, int tag, string name, ref int ierr);///
        public gmshModelSetEntityNameFun gmshModelSetEntityName;
        /// <summary>
        /// Возвращает имя сущности для указанной dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetEntityNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
        private gmshModelGetEntityNameFun gmshModelGetEntityName;
        /// <summary>
        /// Удаляет сущность заданную именем
        /// </summary>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelRemoveEntityNameFun(string name, ref int ierr);///
        public gmshModelRemoveEntityNameFun gmshModelRemoveEntityName;
        /// <summary>
        /// Возвращает ограничивающий параллелепипед для заданных dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность, или -1 использовать все размерности</param>
        /// <param name="tag">[In]Идентификатор, или -1 использовать все идентификаторы</param>
        /// <param name="xmin">[Out]Минимальное значение X</param>
        /// <param name="ymin">[Out]Минимальное значение Y</param>
        /// <param name="zmin">[Out]Минимальное значение Z</param>
        /// <param name="xmax">[Out]Максимальное значение X</param>
        /// <param name="ymax">[Out]Максимальное значение Y</param>
        /// <param name="zmax">[Out]Максимальное значение Z</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelGetBoundingBoxFun(int dim, int tag, ref double xmin,
                                                      ref double ymin, ref double zmin,
                                                      ref double xmax, ref double ymax,
                                                      ref double zmax, ref int ierr);///
        public gmshModelGetBoundingBoxFun gmshModelGetBoundingBox;
        /// <summary>
        /// Возвращает соседей верхнего\нижнего уровня для текущей пары dim : tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="upward">[Out]Идентификаторы верхнего уровня</param>
        /// <param name="upward_n">[Out]Длина вектора идентификаторов</param>
        /// <param name="downward">[Out]Идентификаторы нижнего уровня</param>
        /// <param name="downward_n">[Out]Длина вектора идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetAdjacenciesFun(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                         ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        private gmshModelGetAdjacenciesFun gmshModelGetAdjacencies;
        /// <summary>
        /// Возвращает граничные элементы для заданной пары размерность : идентификатор
        /// </summary>
        /// <param name="dimTags">[In]Массив пара размерность : идентификатор</param>
        /// <param name="dimTags_n">[In]Размер массива пар</param>
        /// <param name="outDimTags">[Out]Граничные элементы внутри пар</param>
        /// <param name="outDimTags_n">[Out]Количество элементов</param>
        /// <param name="combined">[In]Флаг объединения резултатов для пар</param>
        /// <param name="oriented">[In]Флаг наличия знака, который умножается на идентификатор</param>
        /// <param name="recursive">[In]Флаг рекурсии - получить все до точек</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetBoundaryFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                   ref IntPtr outDimTags_n, int combined, int oriented,
                                                   int recursive, ref int ierr);
        private gmshModelGetBoundaryFun gmshModelGetBoundary;
        /// <summary>
        /// Возвращает тип сущности по указанной паре размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="entityType">[Out]Строка типа</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetTypeFun(int dim, int tag, ref IntPtr entityType, ref int ierr);
        private gmshModelGetTypeFun gmshModelGetType;
        /// <summary>
        /// Возвращает максимальную размерность текущего геометрического объекта
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Максимальная размерность</returns>
        public delegate int gmshModelGetDimensionFun(ref int ierr);///
        public gmshModelGetDimensionFun gmshModelGetDimension;
        /// <summary>
        /// Определяет находится ли точка параметрическая или локальная внутри массива
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coord">[In]Масив координат</param>
        /// <param name="coord_n">[In]Количество координат</param>
        /// <param name="parametric">[In]Флаг являяется ли координата параметрической</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Число координат лежащих внутри модели</returns>
        public delegate int gmshModelIsInsideFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                 int parametric, ref int ierr);///
        public gmshModelIsInsideFun gmshModelIsInside;
        /// <summary>
        /// Устанавливает видимость для указанных пар dim:tags геометрических сущностей
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="value">[In]Значение видимости</param>
        /// <param name="recursive">[In]Распространить видимость рекурсивно на граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelSetVisibilityFun(int[] dimTags, IntPtr dimTags_n, int value,
                                                    int recursive, ref int ierr);///
        public gmshModelSetVisibilityFun gmshModelSetVisibility;
        /// <summary>
        /// Связать имя аттрибута со значениями
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="values">[In]Массив значений</param>
        /// <param name="values_n">[In]Размер массива значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelSetAttributeFun(string name, string[] values, IntPtr values_n, ref int ierr);///
        public gmshModelSetAttributeFun gmshModelSetAttribute;
        /// <summary>
        /// Возвратит значения для заданного аттрибута
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="values">[Out]Массив значений аттрибута</param>
        /// <param name="values_n">[Out]Размер массива значений аттрибута</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelGetAttributeFun(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);///
        public gmshModelGetAttributeFun gmshModelGetAttribute;
        /// <summary>
        /// Добавляет идентификаторы поверхностей, формирующих замкнутую оболочку, возвращает идентификатор оболочки
        /// </summary>
        /// <param name="surfaceTags">[In]Массив идентификаторов поверхностей</param>
        /// <param name="surfaceTags_n">[In]Длина массива идентификаторов</param>
        /// <param name="tag">[In]Идентификатор оболочки или -1 для автоматического назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Идентификатор оболочки</returns>
        public delegate int gmshModelGeoAddSurfaceLoopFun(int[] surfaceTags, IntPtr surfaceTags_n, int tag, ref int ierr);///
        public gmshModelGeoAddSurfaceLoopFun gmshModelGeoAddSurfaceLoop;
        /// <summary>
        /// Формирует объем из заданных оболочек, первый элемент массива замкнутая оболочка, следующие элементы
        /// указывают отверстия в геометрии
        /// </summary>
        /// <param name="shellTags">[In]Массив идентификаторов оболочек</param>
        /// <param name="shellTags_n">[In]Длина массива идентификаторов</param>
        /// <param name="tag">[In]Идентификатор объема или -1 для автоматического назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Идентификатор объема</returns>
        public delegate int gmshModelGeoAddVolumeFun(int[] shellTags, IntPtr shellTags_n, int tag, ref int ierr);///
        public gmshModelGeoAddVolumeFun gmshModelGeoAddVolume;
        /// <summary>
        /// Возвращает список имен аттрибутов
        /// </summary>
        /// <param name="names">[Out]Массив имен аттрибутов</param>
        /// <param name="names_n">[Out]Размер массива имен аттрибутов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetAttributeNamesFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelGetAttributeNamesFun gmshModelGetAttributeNames;
        /// <summary>
        /// Удалить имя аттрибута
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelRemoveAttributeFun(string name, ref int ierr);///
        public gmshModelRemoveAttributeFun gmshModelRemoveAttribute;
        /// <summary>
        /// Возвращает физическое имя для заданной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[Out]Физическое имя</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetPhysicalNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
        private gmshModelGetPhysicalNameFun gmshModelGetPhysicalName;
        /// <summary>
        /// Возвращает производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Размер массива параметрических координат</param>
        /// <param name="derivatives">[Out]Массив производных</param>
        /// <param name="derivatives_n">[Out]Размер массива производных</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetDerivativeFun(int dim, int tag, double[] parametricCoord,
                                                    IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                    ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetDerivativeFun gmshModelGetDerivative;
        /// <summary>
        /// Возвращает вторую производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Размер массива параметрических координат</param>
        /// <param name="derivatives">[Out]Массив производных</param>
        /// <param name="derivatives_n">[Out]Размер массива производных</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetSecondDerivativeFun(int dim, int tag, double[] parametricCoord,
                                                          IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                          ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetSecondDerivativeFun gmshModelGetSecondDerivative;
        /// <summary>
        /// Возвращает радиусы кривизны для заданных параметрических координат
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Размер массива параметрических координат</param>
        /// <param name="curvatures">[Out]Массив радиусов кривизны</param>
        /// <param name="curvatures_n">[Out]Размер массива радиусов кривизны</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetCurvatureFun(int dim, int tag, double[] parametricCoord,
                                                   IntPtr parametricCoord_n, ref IntPtr curvatures,
                                                   ref IntPtr curvatures_n, ref int ierr);
        private gmshModelGetCurvatureFun gmshModelGetCurvature;
        /// <summary>
        /// Возвращает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат double</param>
        /// <param name="parameticCoord_n">[In]Размер массива параметрических координат</param>
        /// <param name="coord">[Out]Массив координат точек</param>
        /// <param name="coord_n">[Out]Размер массива координат точек</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetValueFun(int dim, int tag, double[] parametricCoord,
                                               IntPtr parameticCoord_n, ref IntPtr coord,
                                               ref IntPtr coord_n, ref int ierr);
        private gmshModelGetValueFun gmshModelGetValue;
        /// <summary>
        /// Возвращает параметризацию сущности заданной парой размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coord">[In]Массив координат</param>
        /// <param name="coord_n">[In]Размер массива координат</param>
        /// <param name="parametricCoord">[Out]Массив параметрических координат</param>
        /// <param name="parametricCoord_n">[Out]Размер массива параметрических координат</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetParametrizationFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                          ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                          ref int ierr);
        private gmshModelGetParametrizationFun gmshModelGetParametrization;
        /// <summary>
        /// Возвращает минимальные максимальные границы параметризации
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="min">[Out]Массив минимальных границ</param>
        /// <param name="min_n">[Out]Размер массива минимальных границ</param>
        /// <param name="max">[Out]Массив максимальных границ</param>
        /// <param name="max_n">[Out]Размер массива максимальных границ</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelGetParametrizationBoundsFun(int dim, int tag, ref IntPtr min,
                                                               ref IntPtr min_n, ref IntPtr max,
                                                               ref IntPtr max_n, ref int ierr);
        private gmshModelGetParametrizationBoundsFun gmshModelGetParametrizationBounds;
        /// <summary>
        /// Перепараметризирует пару размерность : идентификатор в параметризацию поверхностей
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Размер массива параметрических координат</param>
        /// <param name="surfaceTag">[In]Идентификатор поверхности</param>
        /// <param name="surfaceParametricCoord">[Out]Массив репараметризированных координат</param>
        /// <param name="surfaceParametricCoord_n">[Out]Размер массива репараметризированных координат</param>
        /// <param name="which">[In]Флаг совпадений параметрических поверхностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelReparametrizeOnSurfaceFun(int dim, int tag, double[] parametricCoord,
                                                             IntPtr parametricCoord_n, int surfaceTag,
                                                             ref IntPtr surfaceParametricCoord,
                                                             ref IntPtr surfaceParametricCoord_n,
                                                             int which, ref int ierr);
        private gmshModelReparametrizeOnSurfaceFun gmshModelReparametrizeOnSurface;
        /// <summary>
        /// Возвращает петлю кривых для заданного идентификатора поверхности
        /// </summary>
        /// <param name="surfaceTag">[In]Идентификатор поверхности</param>
        /// <param name="curveLoopTags">[Out]Массив идентификаторов петель кривых</param>
        /// <param name="curveLoopTags_n">[Out]Размер массива идентификаторов петель кривых</param>
        /// <param name="curveTags">[Out]Матрица идентификаторов кривых</param>
        /// <param name="curveTags_n">[Out]Размер внутреннего массива идентификаторов кривых</param>
        /// <param name="curveTags_nn">[Out]Размер внешнего массива идентификаторов кривых</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelOccGetCurveLoopsFun(int surfaceTag, ref IntPtr curveLoopTags,
                                                        ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                        ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                        ref int ierr);
        private gmshModelOccGetCurveLoopsFun gmshModelOccGetCurveLoops;
        /// <summary>
        /// Возвращает петлю поверхностей для заданного идентификатора объема
        /// </summary>
        /// <param name="volumeTag">[In]Идентификатор объема</param>
        /// <param name="surfaceLoopTags">[Out]Массив идентификаторов петель поверхностей</param>
        /// <param name="surfaceLoopTags_n">[Out]Размер массива идентификаторов петель поверхностей</param>
        /// <param name="surfaceTags">[Out]Матрица идентификаторов поверхностей</param>
        /// <param name="surfaceTags_n">[Out]Размер внутреннего массива идентификаторов поверхностей</param>
        /// <param name="surfaceTags_nn">[Out]Размер внешнего массива идентификаторов поверхностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelOccGetSurfaceLoopsFun(int volumeTag, ref IntPtr surfaceLoopTags,
                                                            ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                            ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                            ref int ierr);
        private gmshModelOccGetSurfaceLoopsFun gmshModelOccGetSurfaceLoops;
        #endregion
        #region "Mesh Delegates"
        /// <summary>
        /// Генерирует сетку в текущей размерности
        /// </summary>
        /// <param name="dim">[In]Размерность в которой нужно сгенерировать модель</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshGenerateFun(int dim, ref int ierr);///
        public gmshModelMeshGenerateFun gmshModelMeshGenerate;
        /// <summary>
        /// Перегенерирует сетку преимущественно в четырехугольники
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshRecombineFun(ref int ierr);///
        public gmshModelMeshRecombineFun gmshModelMeshRecombine;
        /// <summary>
        /// Равномерно уплотняет сетку у текущей модели
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshRefineFun(ref int ierr);///
        public gmshModelMeshRefineFun gmshModelMeshRefine;
        /// <summary>
        /// Возвращает все элементы в текущей модели, их идентификаторы и идентификаторы точек внутри элемента
        /// </summary>
        /// <param name="elementTypes">[Out]Массив строк типов элементов</param>
        /// <param name="elementTypes_n">[Out]Длина массива строк типов элементов</param>
        /// <param name="elementTags">[Out]Матрица - массив идентификаторов для каждого типа элемента</param>
        /// <param name="elementTags_n">[Out]Массив - длина массива идентификаторов для типов элементов</param>
        /// <param name="elementTags_nn">[Out]Длина массива типов объектов, дублирует elementTypes_n</param>
        /// <param name="nodeTags">[Out]Матрица - массив идентификаторов точек для типов элементов</param>
        /// <param name="nodeTags_n">[Out]Массив - длина массива идентификаторов точек для типов объекта</param>
        /// <param name="nodeTags_nn">[Out]Длина массива типов объектов, дублирует elementTypes_n</param>
        /// <param name="dim">[In]Размерность для получения элементов, или -1 для получения элементов всех размерностей</param>
        /// <param name="tag">[In]Идентификатор элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetElementsFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                  ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                  ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                  ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                  int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementsFun gmshModelMeshGetElements;
        /// <summary>
        /// Возращает типы элементов, для указанных dim:tag 
        /// </summary>
        /// <param name="elementTypes">[Out]Массив типов элементов</param>
        /// <param name="elementTypes_n">[Out]Размерность массива типа</param>
        /// <param name="dim">[In]Размерность для получения элементов, или -1 для получения элементов всех размерностей</param>
        /// <param name="tag">[In]Идентификатор элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetElementTypesFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                          int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementTypesFun gmshModelMeshGetElementTypes;
        /// <summary>
        /// Возвращает свойства элемента по заданному типу элемента
        /// </summary>
        /// <param name="elementType">[In]Тип элемента</param>
        /// <param name="elementName">[Out]Имя элемента</param>
        /// <param name="dim">[Out]Размерность элемента</param>
        /// <param name="order">[Out]Порядок в сетке</param>
        /// <param name="numNodes">[Out]Число точек в элементе</param>
        /// <param name="localeNodeCoord">[Out]Массив локальных координат элемента</param>
        /// <param name="localeNodeCoord_n">[Out]Размерность массива локальных координат</param>
        /// <param name="numPrimaryNodes">[Out]Количество первичных точек</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetElementPropertiesFun(int elementType, ref IntPtr elementName, ref int dim,
                                                           ref int order, ref int numNodes, ref IntPtr localeNodeCoord,
                                                           ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                           ref int ierr);
        private gmshModelMeshGetElementPropertiesFun gmshModelMeshGetElementProperties;
        /// <summary>
        /// Извлекает координаты точек и их идентификаторы, по заданный параметрам dim:tag элемента
        /// </summary>
        /// <param name="nodeTags">[Out]Массив идентификаторов точек</param>
        /// <param name="nodeTags_n">[Out]Размер массива идентификаторов точек</param>
        /// <param name="coord">[Out]Массив координат точек x,y,z</param>
        /// <param name="coord_n">[Out]Размер массива координат точек</param>
        /// <param name="parametricCoord">[Out]Массив параметрических координат u,u1,u2...</param>
        /// <param name="parametricCoord_n">[Out]Размер массива параметрических координат</param>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Идентификатор элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="includeBoundary">[In]Флаг получения ограничивающего параллелепипеда</param>
        /// <param name="returnParametricCoord">[In]Флаг получения координат</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetNodesFun(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                                       ref IntPtr coord, ref IntPtr coord_n,
                                                       ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                       int dim, int tag, int includeBoundary,
                                                       int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesFun gmshModelMeshGetNodes;
        /// <summary>
        /// Извлекает координаты точек и их идентификаторы, для заданного типа элемента и идентификатора элемента
        /// </summary>
        /// <param name="elementType">[In]Тип элемента</param>
        /// <param name="nodeTags">[Out]Массив идентификаторов точек</param>
        /// <param name="nodeTags_n">[Out]Размер массива идентификаторов точек</param>
        /// <param name="coord">[Out]Массив координат точек x,y,z</param>
        /// <param name="coord_n">[Out]Размер массива координат точек</param>
        /// <param name="parametricCoord">[Out]Массив параметрических координат u,u1,u2...</param>
        /// <param name="parametricCoord_n">[Out]Размер массива параметрических координат</param>
        /// <param name="tag">[In]Идентификатор элемента</param>
        /// <param name="returnParametricCoord">[In]Возвращать параметрические координаты?</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetNodesByElementTypeFun(int elementType, ref IntPtr nodeTags,
                                                                    ref IntPtr nodeTags_n, ref IntPtr coord,
                                                                    ref IntPtr coord_n, ref IntPtr parametricCoord,
                                                                    ref IntPtr parametricCoord_n, int tag,
                                                                    int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesByElementTypeFun gmshModelMeshGetNodesByElementType;
        /// <summary>
        /// Задает количество опорных точек на кривой, по которой будет строиться сетка
        /// </summary>
        /// <param name="tag">[In]Идентификатор кривой</param>
        /// <param name="numNodes">[In]Число точек на кривой</param>
        /// <param name="meshType">[In]Алгоритм распределения точек</param>
        /// <param name="coef">[In]Коэффициент прогрессии</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshSetTransfiniteCurveFun(int tag, int numNodes, string meshType,
                                                                  double coef, ref int ierr);///
        public gmshModelMeshSetTransfiniteCurveFun gmshModelMeshSetTransfiniteCurve;
        /// <summary>
        /// Задает количество опорных точек для поверхности
        /// </summary>
        /// <param name="tag">[In]Идентифкатор поверхности</param>
        /// <param name="arrangement">[In]Расположение "Left", "Right", "AlternateLeft", "AlternateRight"</param>
        /// <param name="cornerTags">[In]Массив идентификаторов углов 3,4 и более по которым будет вестись инетрполяция</param>
        /// <param name="cornerTags_n">[In]Размер массива идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshSetTransfiniteSurfaceFun(int tag, string arrangement, int[] cornerTags,
                                                                       IntPtr cornerTags_n, ref int ierr);///
        public gmshModelMeshSetTransfiniteSurfaceFun gmshModelMeshSetTransfiniteSurface;
        /// <summary>
        /// Задает количество опорных точек для объема
        /// </summary>
        /// <param name="tag">[In]Идентификатор объема</param>
        /// <param name="cornerTags">[In]Массив идентификаторов углов 6,8 по которым будет вестись инетрполяция</param>
        /// <param name="cornerTags_n">[In]Размер массива идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshSetTransfiniteVolumeFun(int tag, int[] cornerTags, IntPtr cornerTags_n,
                                                                  ref int ierr);///
        public gmshModelMeshSetTransfiniteVolumeFun gmshModelMeshSetTransfiniteVolume;
        /// <summary>
        /// Находит элементы сетки по переданной координате x,y,z
        /// </summary>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="elementTags">[Out]Массив найденных элементов</param>
        /// <param name="elementTags_n">[Out]Размер массива</param>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="strict">[In]Строгий поиск?</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetElementsByCoordinatesFun(double x, double y, double z,
                                                                       ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                                       int dim, int strict, ref int ierr);
        private gmshModelMeshGetElementsByCoordinatesFun gmshModelMeshGetElementsByCoordinates;
        /// <summary>
        /// Получает локальные координаты элемента с заданным идентификатором
        /// </summary>
        /// <param name="elementTag">[In]Идентификатор элемента</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="u">[Out]Параметрические координаты u</param>
        /// <param name="v">[Out]Параметрические координаты v</param>
        /// <param name="w">[Out]Параметрические координаты w</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshGetLocalCoordinatesInElementFun(long elementTag, double x, double y, double z,
                                                                                   ref double u, ref double v, ref double w,
                                                                                   ref int ierr);///
        public gmshModelMeshGetLocalCoordinatesInElementFun gmshModelMeshGetLocalCoordinatesInElement;
        /// <summary>
        /// Перестроить кеш точек
        /// </summary>
        /// <param name="onlyIfNecessary">[In]Флаг удаления</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshRebuildNodeCacheFun(int onlyIfNecessary, ref int ierr);///
        public gmshModelMeshRebuildNodeCacheFun gmshModelMeshRebuildNodeCache;
        /// <summary>
        /// Перестроить кеш элементов
        /// </summary>
        /// <param name="onlyIfNecessary">[In]Флаг удаления</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshRebuildElementCacheFun(int onlyIfNecessary, ref int ierr);///
        public gmshModelMeshRebuildElementCacheFun gmshModelMeshRebuildElementCache;
        /// <summary>
        /// Удаляет сетку свзанную с массивом пар dim:tag, включая точки
        /// </summary>
        /// <param name="dimTags">[In]Пары dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива dim:tag</param>
        /// <param name="ierr">[Out]Флаг ошибок</param>
        public delegate void gmshModelMeshClearFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);///
        public gmshModelMeshClearFun gmshModelMeshClear;
        /// <summary>
        /// Задать видимость элементов
        /// </summary>
        /// <param name="elementTags">[In]Массив элементов</param>
        /// <param name="elementTags_n">[In]Размер массива элементов</param>
        /// <param name="value">[In]Значение видимости</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshSetVisibilityFun(long[] elementTags, IntPtr elementTags_n,
                                                            int value, ref int ierr);///
        public gmshModelMeshSetVisibilityFun gmshModelMeshSetVisibility;
        /// <summary>
        /// Устанавливает алгоритм триангуляции для сетки
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="val">[In]Численное значение поля алгоритма</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshSetAlgorithmFun(int dim, int tag, int val, ref int ierr);///
        public gmshModelMeshSetAlgorithmFun gmshModelMeshSetAlgorithm;
        /// <summary>
        /// Получить качество элемента
        /// </summary>
        /// <param name="elementTags">[In]Массив идентификаторов элементов</param>
        /// <param name="elementTags_n">[In]Размер массива идентификаторов</param>
        /// <param name="elementsQuality">[Out]Массив качеств элементов</param>
        /// <param name="elementQuality_n">[Out]Размер массив качеств элементов</param>
        /// <param name="qualityName">[In]Строка-запроса (minDetJac,maxDetJac,minSJ, minSICN, minSIGE, gamma,
        /// innerRadius, outerRadius, minIsotropy, angleShape, minEdge, maxEdge, volume) </param>
        /// <param name="task">[In]Task</param>
        /// <param name="numTasks">[In]NumTask</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshGetElementQualitiesFun(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                      ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
                                                                      string qualityName, IntPtr task,
                                                                      IntPtr numTasks, ref int ierr);
        private gmshModelMeshGetElementQualitiesFun gmshModelMeshGetElementQualities;
        #endregion
        #region "Mesh Field Delegates"
        /// <summary>
        /// Добавить объект MeshField
        /// </summary>
        /// <param name="fieldType">[In]Тип поля</param>
        /// <param name="tag">[In]Идентификатор (может быть -1, выделится автоматически)</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Идентификатор объекта Field</returns>
        public delegate int gmshModelMeshFieldAddFun(string fieldType, int tag, ref int ierr);///
        public gmshModelMeshFieldAddFun gmshModelMeshFieldAdd;
        /// <summary>
        /// Удалить объект MeshField
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldRemoveFun(int tag, ref int ierr);///
        public gmshModelMeshFieldRemoveFun gmshModelMeshFieldRemove;
        /// <summary>
        /// Возвращает список объектов MeshField
        /// </summary>
        /// <param name="tags">[Out]Идентификаторы</param>
        /// <param name="tags_n">[Out]Количество идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshFieldListFun(ref IntPtr tags, ref IntPtr tags_n, ref int ierr);
        private gmshModelMeshFieldListFun gmshModelMeshFieldList;
        /// <summary>
        /// Возвращает тип поля по заданному идентификатору
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="fieldType">[Out]Тип поля</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshFieldGetTypeFun(int tag, ref IntPtr fieldType, ref int ierr);
        private gmshModelMeshFieldGetTypeFun gmshModelMeshFieldGetType;
        /// <summary>
        /// Задает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значение</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldSetNumberFun(int tag, string option, double value, ref int ierr);///
        public gmshModelMeshFieldSetNumberFun gmshModelMeshFieldSetNumber;
        /// <summary>
        /// Возвращает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значение</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldGetNumberFun(int tag, string option, ref double value, ref int ierr);///
        public gmshModelMeshFieldGetNumberFun gmshModelMeshFieldGetNumber;
        /// <summary>
        /// Задает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значения опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldSetStringFun(int tag, string option, string value, ref int ierr);///
        public gmshModelMeshFieldSetStringFun gmshModelMeshFieldSetString;
        /// <summary>
        /// Возвращает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[Out]Набор значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshFieldGetStringFun(int tag, string option, ref IntPtr value, ref int ierr);
        private gmshModelMeshFieldGetStringFun gmshModelMeshFieldGetString;
        /// <summary>
        /// Задает список значений для опции 
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="values">[In]Массив значений</param>
        /// <param name="values_n">[In]количество значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldSetNumbersFun(int tag, string option, double[] values,
                                                              IntPtr values_n, ref int ierr);///
        public gmshModelMeshFieldSetNumbersFun gmshModelMeshFieldSetNumbers;
        /// <summary>
        /// Возвращает список значений для опции 
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="values">[Out]Массив значений типа double</param>
        /// <param name="values_n">[Out]Количество значений типа double</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshModelMeshFieldGetNumbersFun(int tag, string option, ref IntPtr values,
                                                              ref IntPtr values_n, ref int ierr);
        private gmshModelMeshFieldGetNumbersFun gmshModelMeshFieldGetNumbers;
        /// <summary>
        /// Установить поле как BackgroundMesh
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldSetAsBackgroundMeshFun(int tag, ref int ierr);///
        public gmshModelMeshFieldSetAsBackgroundMeshFun gmshModelMeshFieldSetAsBackgroundMesh;
        /// <summary>
        /// Установить поле как BoundaryLayer
        /// </summary>
        /// <param name="tag">[In]Идентифкатор поля</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshModelMeshFieldSetAsBoundaryLayerFun(int tag, ref int ierr);///
        public gmshModelMeshFieldSetAsBoundaryLayerFun gmshModelMeshFieldSetAsBoundaryLayer;
        #endregion
        #region "Logger Delegates"
        /// <summary>
        /// Начать запись журнала
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshLoggerStartFun(ref int ierr);///
        public gmshLoggerStartFun gmshLoggerStart;
        /// <summary>
        /// Остановить запись журнала
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        public delegate void gmshLoggerStopFun(ref int ierr);///
        public gmshLoggerStopFun gmshLoggerStop;
        /// <summary>
        /// Вернуть записи в журнале
        /// </summary>
        /// <param name="log">[Out]Массив записей</param>
        /// <param name="log_n">[Out]Размер массива записей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshLoggerGetFun(ref IntPtr log, ref IntPtr log_n, ref int ierr);
        private gmshLoggerGetFun gmshLoggerGet;
        /// <summary>
        /// Вернуть последнюю запись об ошибке в журнале
        /// </summary>
        /// <param name="error">[Out]Запись ошибки</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        private delegate void gmshLoggerGetLastErrorFun(ref IntPtr error, ref int ierr);
        private gmshLoggerGetLastErrorFun gmshLoggerGetLastError;
        #endregion
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dllToLoad">Путь до DLL</param>
        public GmshController(string dllToLoad)
        {
            DLL = GmshKernel.GmshKernel.LoadLibrary(dllToLoad);
            #region "Get Initialization delegates"
            gmshInitialize = GetFunction<gmshInitializeFun>("gmshInitialize");
            gmshIsInitialized = GetFunction<gmshIsInitializedFun>("gmshIsInitialized");
            gmshFinalize = GetFunction<gmshFinalizeFun>("gmshFinalize");
            #endregion
            #region "Get Memory delegates"
            gmshMalloc = GetFunction<gmshMallocFun>("gmshMalloc");
            gmshFree = GetFunction<gmshFreeFun>("gmshFree");
            #endregion
            #region "Get General delegates"
            gmshOptionSetNumber = GetFunction<gmshOptionSetNumberFun>("gmshOptionSetNumber");
            gmshOptionGetNumber = GetFunction<gmshOptionGetNumberFun>("gmshOptionGetNumber");
            gmshOptionSetString = GetFunction<gmshOptionSetStringFun>("gmshOptionSetString");
            //Private members
            gmshOptionGetString = GetFunction<gmshOptionGetStringFun>("gmshOptionGetString");
            #endregion
            #region "Get Plugin delegates"
            gmshPluginSetNumber = GetFunction<gmshPluginSetNumberFun>("gmshPluginSetNumber");
            gmshPluginSetString = GetFunction<gmshPluginSetStringFun>("gmshPluginSetString");
            gmshPluginRun = GetFunction<gmshPluginRunFun>("gmshPluginRun");
            #endregion
            #region "Get IO delegates"
            gmshOpen = GetFunction<gmshOpenFun>("gmshOpen");
            gmshMerge = GetFunction<gmshMergeFun>("gmshMerge");
            gmshWrite = GetFunction<gmshWriteFun>("gmshWrite");
            #endregion
            #region "Get Model delegates"
            gmshModelSetFileName = GetFunction<gmshModelSetFileNameFun>("gmshModelSetFileName");
            gmshModelAdd = GetFunction<gmshModelAddFun>("gmshModelAdd");
            gmshModelRemove = GetFunction<gmshModelRemoveFun>("gmshModelRemove");
            gmshClear = GetFunction<gmshClearFun>("gmshClear");
            gmshModelSetCurrent = GetFunction<gmshModelSetCurrentFun>("gmshModelSetCurrent");
            gmshModelOccSynchronize = GetFunction<gmshModelOccSynchronizeFun>("gmshModelOccSynchronize");
            gmshModelGeoSynchronize = GetFunction<gmshModelGeoSynchronizeFun>("gmshModelGeoSynchronize");
            //Private members
            gmshModelGetFileName = GetFunction<gmshModelGetFileNameFun>("gmshModelGetFileName");
            gmshModelList = GetFunction<gmshModelListFun>("gmshModelList");
            gmshModelGetCurrent = GetFunction<gmshModelGetCurrentFun>("gmshModelGetCurrent");
            gmshModelOccImportShapes = GetFunction<gmshModelOccImportShapesFun>("gmshModelOccImportShapes");
            #endregion
            #region "Get Geometry delegates"
            gmshModelGetEntities = GetFunction<gmshModelGetEntitiesFun>("gmshModelGetEntities");
            gmshModelOccGetEntities = GetFunction<gmshModelOccGetEntitiesFun>("gmshModelOccGetEntities");
            gmshModelRemoveEntities = GetFunction<gmshModelRemoveEntitiesFun>("gmshModelRemoveEntities");
            gmshModelOccRemove = GetFunction<gmshModelOccRemoveFun>("gmshModelOccRemove");
            gmshModelSetEntityName = GetFunction<gmshModelSetEntityNameFun>("gmshModelSetEntityName");
            gmshModelGetEntityName = GetFunction<gmshModelGetEntityNameFun>("gmshModelGetEntityName");
            gmshModelRemoveEntityName = GetFunction<gmshModelRemoveEntityNameFun>("gmshModelRemoveEntityName");
            gmshModelGetBoundingBox = GetFunction<gmshModelGetBoundingBoxFun>("gmshModelGetBoundingBox");
            gmshModelGetAdjacencies = GetFunction<gmshModelGetAdjacenciesFun>("gmshModelGetAdjacencies");
            gmshModelGetBoundary = GetFunction<gmshModelGetBoundaryFun>("gmshModelGetBoundary");
            gmshModelGetType = GetFunction<gmshModelGetTypeFun>("gmshModelGetType");
            gmshModelGetDimension = GetFunction<gmshModelGetDimensionFun>("gmshModelGetDimension");
            gmshModelIsInside = GetFunction<gmshModelIsInsideFun>("gmshModelIsInside");
            gmshModelSetVisibility = GetFunction<gmshModelSetVisibilityFun>("gmshModelSetVisibility");
            gmshModelSetAttribute = GetFunction<gmshModelSetAttributeFun>("gmshModelSetAttribute");
            gmshModelGetAttribute = GetFunction<gmshModelGetAttributeFun>("gmshModelGetAttribute");
            gmshModelGeoAddSurfaceLoop = GetFunction<gmshModelGeoAddSurfaceLoopFun>("gmshModelGeoAddSurfaceLoop");
            gmshModelGeoAddVolume = GetFunction<gmshModelGeoAddVolumeFun>("gmshModelGeoAddVolume");
            gmshModelGetAttributeNames = GetFunction<gmshModelGetAttributeNamesFun>("gmshModelGetAttributeNames");
            gmshModelRemoveAttribute = GetFunction<gmshModelRemoveAttributeFun>("gmshModelRemoveAttribute");
            gmshModelGetPhysicalName = GetFunction<gmshModelGetPhysicalNameFun>("gmshModelGetPhysicalName");
            gmshModelGetDerivative = GetFunction<gmshModelGetDerivativeFun>("gmshModelGetDerivative");
            gmshModelGetSecondDerivative = GetFunction<gmshModelGetSecondDerivativeFun>("gmshModelGetSecondDerivative");
            gmshModelGetCurvature = GetFunction<gmshModelGetCurvatureFun>("gmshModelGetCurvature");
            gmshModelGetValue = GetFunction<gmshModelGetValueFun>("gmshModelGetValue");
            gmshModelGetParametrization = GetFunction<gmshModelGetParametrizationFun>("gmshModelGetParametrization");
            gmshModelGetParametrizationBounds = GetFunction<gmshModelGetParametrizationBoundsFun>("gmshModelGetParametrizationBounds");
            gmshModelReparametrizeOnSurface = GetFunction<gmshModelReparametrizeOnSurfaceFun>("gmshModelReparametrizeOnSurface");
            gmshModelOccGetCurveLoops = GetFunction<gmshModelOccGetCurveLoopsFun>("gmshModelOccGetCurveLoops");
            gmshModelOccGetSurfaceLoops = GetFunction<gmshModelOccGetSurfaceLoopsFun>("gmshModelOccGetSurfaceLoops");
            #endregion
            #region "Get Mesh delegates"
            gmshModelMeshGenerate = GetFunction<gmshModelMeshGenerateFun>("gmshModelMeshGenerate");
            gmshModelMeshRecombine = GetFunction<gmshModelMeshRecombineFun>("gmshModelMeshRecombine");
            gmshModelMeshRefine = GetFunction<gmshModelMeshRefineFun>("gmshModelMeshRefine");
            gmshModelMeshGetElements = GetFunction<gmshModelMeshGetElementsFun>("gmshModelMeshGetElements");
            gmshModelMeshGetElementTypes = GetFunction<gmshModelMeshGetElementTypesFun>("gmshModelMeshGetElementTypes");
            gmshModelMeshGetElementProperties = GetFunction<gmshModelMeshGetElementPropertiesFun>("gmshModelMeshGetElementProperties");
            gmshModelMeshGetNodes = GetFunction<gmshModelMeshGetNodesFun>("gmshModelMeshGetNodes");
            gmshModelMeshGetNodesByElementType = GetFunction<gmshModelMeshGetNodesByElementTypeFun>("gmshModelMeshGetNodesByElementType");
            gmshModelMeshSetTransfiniteCurve = GetFunction<gmshModelMeshSetTransfiniteCurveFun>("gmshModelMeshSetTransfiniteCurve");
            gmshModelMeshSetTransfiniteSurface = GetFunction<gmshModelMeshSetTransfiniteSurfaceFun>("gmshModelMeshSetTransfiniteSurface");
            gmshModelMeshSetTransfiniteVolume = GetFunction<gmshModelMeshSetTransfiniteVolumeFun>("gmshModelMeshSetTransfiniteVolume");
            gmshModelMeshGetElementsByCoordinates = GetFunction<gmshModelMeshGetElementsByCoordinatesFun>("gmshModelMeshGetElementsByCoordinates");
            gmshModelMeshGetLocalCoordinatesInElement = GetFunction<gmshModelMeshGetLocalCoordinatesInElementFun>("gmshModelMeshGetLocalCoordinatesInElement");
            gmshModelMeshRebuildNodeCache = GetFunction<gmshModelMeshRebuildNodeCacheFun>("gmshModelMeshRebuildNodeCache");
            gmshModelMeshRebuildElementCache = GetFunction<gmshModelMeshRebuildElementCacheFun>("gmshModelMeshRebuildElementCache");
            gmshModelMeshClear = GetFunction<gmshModelMeshClearFun>("gmshModelMeshClear");
            gmshModelMeshSetVisibility = GetFunction<gmshModelMeshSetVisibilityFun>("gmshModelMeshSetVisibility");
            gmshModelMeshSetAlgorithm = GetFunction<gmshModelMeshSetAlgorithmFun>("gmshModelMeshSetAlgorithm");
            gmshModelMeshGetElementQualities = GetFunction<gmshModelMeshGetElementQualitiesFun>("gmshModelMeshGetElementQualities");
            #endregion
            #region "Get Mesh Field delegates"
            gmshModelMeshFieldAdd = GetFunction<gmshModelMeshFieldAddFun>("gmshModelMeshFieldAdd");
            gmshModelMeshFieldRemove = GetFunction<gmshModelMeshFieldRemoveFun>("gmshModelMeshFieldRemove");
            gmshModelMeshFieldList = GetFunction<gmshModelMeshFieldListFun>("gmshModelMeshFieldList");
            gmshModelMeshFieldGetType = GetFunction<gmshModelMeshFieldGetTypeFun>("gmshModelMeshFieldGetType");
            gmshModelMeshFieldSetNumber = GetFunction<gmshModelMeshFieldSetNumberFun>("gmshModelMeshFieldSetNumber");
            gmshModelMeshFieldGetNumber = GetFunction<gmshModelMeshFieldGetNumberFun>("gmshModelMeshFieldGetNumber");
            gmshModelMeshFieldGetString = GetFunction<gmshModelMeshFieldGetStringFun>("gmshModelMeshFieldGetString");
            gmshModelMeshFieldSetString = GetFunction<gmshModelMeshFieldSetStringFun>("gmshModelMeshFieldSetString");
            gmshModelMeshFieldSetNumbers = GetFunction<gmshModelMeshFieldSetNumbersFun>("gmshModelMeshFieldSetNumbers");
            gmshModelMeshFieldGetNumbers = GetFunction<gmshModelMeshFieldGetNumbersFun>("gmshModelMeshFieldGetNumbers");
            gmshModelMeshFieldSetAsBackgroundMesh = GetFunction<gmshModelMeshFieldSetAsBackgroundMeshFun>("gmshModelMeshFieldSetAsBackgroundMesh");
            gmshModelMeshFieldSetAsBoundaryLayer = GetFunction<gmshModelMeshFieldSetAsBoundaryLayerFun>("gmshModelMeshFieldSetAsBoundaryLayer");
            #endregion
            #region "Get Logger delegates"
            gmshLoggerStart = GetFunction<gmshLoggerStartFun>("gmshLoggerStart");
            gmshLoggerStop = GetFunction<gmshLoggerStopFun>("gmshLoggerStop");
            gmshLoggerGet = GetFunction<gmshLoggerGetFun>("gmshLoggerGet");
            gmshLoggerGetLastError = GetFunction<gmshLoggerGetLastErrorFun>("gmshLoggerGetLastError");
            #endregion
            var argv = string.Empty;
            var ierr = 0;
            gmshInitialize(0, ref argv, 0, 0, ref ierr);
        }
        /// <summary>
        /// Возвращает делегат для имени функции 
        /// </summary>
        /// <typeparam name="T">Тип делегата</typeparam>
        /// <param name="functionName">Имя функции</param>
        /// <returns>Делегат</returns>
        public T GetFunction<T>(string functionName)
        {
            var funPtr = GmshKernel.GmshKernel.GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }
        #region "General managed methods"
        /// <summary>
        /// Возвращает строку параметров для категории опций
        /// </summary>
        /// <param name="name">Имя категории опций</param>
        /// <param name="value">Значение опции</param>
        /// <returns>Успешность вызова метода</returns>
        public bool OptionGetString(string name, out string value)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            value = null;
            gmshOptionGetString(name, ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                value = GetStringPtr(unmanagedString);
                gmshFree(unmanagedString);
            }

            return status;
        }
        #endregion
        #region "Model managed methods"
        /// <summary>
        /// Возвращает путь до файла ассоциированный с данной моделью
        /// </summary>
        /// <param name="fileName">Путь до файла ассоциированный с данной моделью</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetFileName(out string fileName)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            fileName = null;
            gmshModelGetFileName(ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                fileName = GetStringPtr(unmanagedString);
                gmshFree(unmanagedString);
            }

            return status;
        }
        /// <summary>
        /// Возвращает список моделей в gmsh API
        /// </summary>
        /// <param name="modelList">Массив строк имен моделей</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelList(out string[] modelList)
        {
            IntPtr names = IntPtr.Zero;
            IntPtr names_n = IntPtr.Zero;
            var ierr = 0;
            modelList = null;
            gmshModelList(ref names, ref names_n, ref ierr);
            var status = ierr == 0;
            if (status)
                modelList = GetArrayOfString(names, names_n);
            return status;
        }
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <param name="current">Имя текущей модели</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetCurrent(out string current)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            current = null;
            gmshModelGetCurrent(ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                current = GetStringPtr(unmanagedString);
                gmshFree(unmanagedString);
            }

            return status;
        }
        #endregion
        #region "Geometry managed methods"
        /// <summary>
        /// Возвращает список всех пар размерность:идентификатор для заданной размерности
        /// </summary>
        /// <param name="dimTags">Пары dim:tag</param>
        /// <param name="dim">Заданная размерность</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetGeometryEntities(out int[] dimTags, int dim)
        {
            var ierr = 0;
            IntPtr dTags, dimTags_n;
            dTags = dimTags_n = IntPtr.Zero;
            dimTags = null;
            string fileName;
            ModelGetFileName(out fileName);
            var ext = Path.GetExtension(fileName);
            if (geomExtensions.Contains(ext))
                gmshModelOccGetEntities(ref dTags, ref dimTags_n, dim, ref ierr);
            else
                gmshModelGetEntities(ref dTags, ref dimTags_n, dim, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(dTags, out dimTags, dimTags_n);
            return status;
        }
        /// <summary>
        /// Возвращает имя сущности для указанной dim:tag
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="name">Имя</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetGeometryEntityName(int dim, int tag, out string name)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            name = null;
            gmshModelGetEntityName(dim, tag, ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                name = GetStringPtr(unmanagedString);
                gmshFree(unmanagedString);
            }

            return status;
        }
        /// <summary>
        /// Возвращает соседей верхнего\нижнего уровня для текущей пары dim : tag
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="upward">Соседи верхнего уровня</param>
        /// <param name="downward">Соседи нижнего уровня</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetAdjacencies(int dim, int tag, out int[] upward, out int[] downward)
        {
            var ierr = 0;
            IntPtr upw, upw_n, downw, downw_n;
            upw = upw_n = downw = downw_n = IntPtr.Zero;
            upward = null;
            downward = null;
            gmshModelGetAdjacencies(dim, tag, ref upw, ref upw_n, ref downw, ref downw_n, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(downw, out downward, downw_n);
                GetObjectsData(upw, out upward, upw_n);
            }
            return status;
        }
        /// <summary>
        /// Возвратит значения для заданного аттрибута
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        /// <param name="attributes">Значения аттрибута</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetAttribute(string name, out string[] attributes)
        {
            var ierr = 0;
            IntPtr values = IntPtr.Zero, values_n = IntPtr.Zero;
            attributes = null;
            gmshModelGetAttribute(name, ref values, ref values_n, ref ierr);
            var status = ierr == 0;
            if (status)
                attributes = GetArrayOfString(values, values_n);
            return status;
        }
        /// <summary>
        /// Возвращает список имен аттрибутов
        /// </summary>
        /// <param name="names">Список имен аттрибутов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetAttributeNames(out string[] names)
        {
            var ierr = 0;
            IntPtr nms = IntPtr.Zero, nms_n = IntPtr.Zero;
            names = null;
            gmshModelGetAttributeNames(ref nms, ref nms_n, ref ierr);
            var status = ierr == 0;
            if (status)
                names = GetArrayOfString(nms, nms_n);
            return status;
        }
        /// <summary>
        /// Возвращает производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <param name="derivative">Производные</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetDerivative(int dim, int tag, double[] parametricCoord, out double[] derivative)
        {
            var ierr = 0;
            var der = IntPtr.Zero;
            var der_n = IntPtr.Zero;
            derivative = null;
            gmshModelGetDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                                   ref der, ref der_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(der, out derivative, der_n);
            return status;
        }
        /// <summary>
        /// Возвращает вторую производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <param name="derivative">Производные</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetSecondDerivative(int dim, int tag, double[] parametricCoord, out double[] derivative)
        {
            var ierr = 0;
            var der = IntPtr.Zero;
            var der_n = IntPtr.Zero;
            derivative = null;
            gmshModelGetSecondDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                                         ref der, ref der_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(der, out derivative, der_n);
            return status;
        }
        /// <summary>
        /// Возвращает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <param name="coord">Локальные координаты</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetValue(int dim, int tag, double[] parametricCoord, out double[] coord)
        {
            var ierr = 0;
            IntPtr locCoord, locCoord_n;
            locCoord = locCoord_n = IntPtr.Zero;
            coord = null;
            gmshModelGetValue(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                              ref locCoord, ref locCoord_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(locCoord, out coord, locCoord_n);
            return status;
        }
        /// <summary>
        /// Возвращает параметризацию сущности заданной парой размерность : идентификатор
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="coord">Локальные координаты</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetParametrization(int dim, int tag, double[] coord, out double[] parametricCoord)
        {
            var ierr = 0;
            IntPtr pCoord, pCoord_n;
            pCoord = pCoord_n = IntPtr.Zero;
            parametricCoord = null;
            gmshModelGetParametrization(dim, tag, coord, (IntPtr)coord.Length, ref pCoord, ref pCoord_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(pCoord, out parametricCoord, pCoord_n);
            return status;
        }
        /// <summary>
        /// Возвращает минимальные максимальные границы параметризации
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="min">Минимальные границы</param>
        /// <param name="max">Максимальные границы</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelGetParametrizationBounds(int dim, int tag, out double[] min, out double[] max)
        {
            var ierr = 0;
            IntPtr minBounds, minBounds_n;
            IntPtr maxBounds, maxBounds_n;
            minBounds = minBounds_n = maxBounds = maxBounds_n = IntPtr.Zero;
            min = null;
            max = null;
            gmshModelGetParametrizationBounds(dim, tag, ref minBounds, ref minBounds_n,
                                              ref maxBounds, ref maxBounds_n, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(minBounds, out min, minBounds_n);
                GetObjectsData(maxBounds, out max, maxBounds_n);
            }
            return status;
        }
        #endregion
        #region "Mesh managed methods"
        /// <summary>
        /// Возвращает все элементы в текущей модели, их идентификаторы и идентификаторы точек внутри элемента
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="elementTypes">Типы элементов</param>
        /// <param name="elementTags">Идентификаторы элементов</param>
        /// <param name="nodeTags">Идентификаторы узлов элементов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetElements(int dim, int tag, out int[] elementTypes,
                                         out long[][] elementTags, out long[][] nodeTags)
        {
            IntPtr elemTypes, elemTypes_n, elemTags, elemTags_n, elemTags_nn;
            IntPtr nodTags, nodTags_n, nodTags_nn;
            elemTypes = elemTypes_n = elemTags = elemTags_n = elemTags_nn = IntPtr.Zero;
            nodTags = nodTags_n = nodTags_nn = IntPtr.Zero;
            var ierr = 0;
            elementTypes = null;
            elementTags = nodeTags = null;
            gmshModelMeshGetElements(ref elemTypes, ref elemTypes_n, ref elemTags,
                                     ref elemTags_n, ref elemTags_nn, ref nodTags,
                                     ref nodTags_n, ref nodTags_nn, dim, tag, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(elemTypes, out elementTypes, elemTypes_n);
                FillTwoDimArray(elemTags, elemTags_n, elemTags_nn, out elementTags);
                FillTwoDimArray(nodTags, nodTags_n, nodTags_nn, out nodeTags);
            }
            return status;
        }
        /// <summary>
        /// Возращает типы элементов, для указанных dim:tag 
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="elementTypes">Типы элементов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetElementTypes(int dim, int tag, out int[] elementTypes)
        {
            IntPtr elemTypes = IntPtr.Zero;
            IntPtr elemTypes_n = IntPtr.Zero;
            var ierr = 0;
            elementTypes = null;
            gmshModelMeshGetElementTypes(ref elemTypes, ref elemTypes_n, dim, tag, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(elemTypes, out elementTypes, elemTypes_n);
            return status;
        }
        /// <summary>
        /// Возвращает свойства элемента по заданному типу элемента
        /// </summary>
        /// <param name="elementTypes">Типы элементов</param>
        /// <param name="elemProperties">Свойства элементов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetElementProperties(int[] elementTypes, out ElementProperties[] elemProperties)
        {
            IntPtr localeNodeCoord, localeNodeCoord_n, elementName;
            localeNodeCoord = localeNodeCoord_n = elementName = IntPtr.Zero;
            var ierr = 0;
            var status = true;
            elemProperties = new ElementProperties[elementTypes.Length];
            for (var i = 0; i < elementTypes.Length; ++i)
            {
                elemProperties[i] = new ElementProperties();
                gmshModelMeshGetElementProperties(elementTypes[i], ref elementName, ref elemProperties[i].dim,
                                                  ref elemProperties[i].order, ref elemProperties[i].numNodes,
                                                  ref localeNodeCoord, ref localeNodeCoord_n,
                                                  ref elemProperties[i].numPrimaryNodes, ref ierr);
                status = ierr == 0;
                if (!status)
                {
                    elemProperties = null;
                    break;
                }
                elemProperties[i].elementName = GetStringPtr(elementName);
                gmshFree(elementName);
                GetObjectsData(localeNodeCoord, out elemProperties[i].coords, localeNodeCoord_n);
                elemProperties[i].coords_n = elemProperties[i].coords.Length;
            }
            return status;
        }
        /// <summary>
        /// Извлекает координаты точек и их идентификаторы, по заданный параметрам dim:tag элемента
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="includeBoundary">Включать граничные точки</param>
        /// <param name="returnParametric">Возвращать параметрические координаты</param>
        /// <param name="nodeTags">Идентификаторы узлов</param>
        /// <param name="coords">Локальные координаты</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetNodes(int dim, int tag, bool includeBoundary, bool returnParametric,
                                      out long[] nodeTags, out double[] coords, out double[] parametricCoord)
        {
            IntPtr nodeTagsInt, nodeTagsInt_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTagsInt = nodeTagsInt_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            var ierr = 0;
            coords = parametricCoord = null;
            nodeTags = null;
            var includeBound = Convert.ToInt32(includeBoundary);
            var retParam = Convert.ToInt32(returnParametric);
            gmshModelMeshGetNodes(ref nodeTagsInt, ref nodeTagsInt_n, ref coord, ref coord_n,
                                  ref paramCoord, ref paramCoord_n, dim, tag, includeBound, retParam, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(nodeTagsInt, out nodeTags, nodeTagsInt_n);
                GetObjectsData(coord, out coords, coord_n);
                if (returnParametric)
                    GetObjectsData(paramCoord, out parametricCoord, paramCoord_n);
            }
            return status;
        }
        /// <summary>
        /// Извлекает координаты точек и их идентификаторы, для заданного типа элемента и идентификатора элемента
        /// </summary>
        /// <param name="elementType">Тип элемента</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="returnParametric">Возвращать параметрические координаты</param>
        /// <param name="nodeTags">Идентификаторы узлов</param>
        /// <param name="coords">Локальные координаты</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetNodesByElementType(int elementType, int tag, bool returnParametric,
                                                   out long[] nodeTags, out double[] coords, out double[] parametricCoord)
        {
            IntPtr nodeTagsInt, nodeTagsInt_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTagsInt = nodeTagsInt_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            var ierr = 0;
            coords = parametricCoord = null;
            nodeTags = null;
            var retParam = Convert.ToInt32(returnParametric);
            gmshModelMeshGetNodesByElementType(elementType, ref nodeTagsInt, ref nodeTagsInt_n, ref coord, ref coord_n,
                                               ref paramCoord, ref paramCoord_n, tag, retParam, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(nodeTagsInt, out nodeTags, nodeTagsInt_n);
                GetObjectsData(coord, out coords, coord_n);
                if (returnParametric)
                    GetObjectsData(paramCoord, out parametricCoord, paramCoord_n);
            }
            return ierr == 0;
        }
        /// <summary>
        /// Находит элементы сетки по переданной координате x,y,z
        /// </summary>
        /// <param name="x">Локальный x</param>
        /// <param name="y">Локальный y</param>
        /// <param name="z">Локальный z</param>
        /// <param name="dim">Размерность</param>
        /// <param name="strict">Строгое соответствие</param>
        /// <param name="elementTags">Идентификаторы элементов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetElementsByCoordinates(double x, double y, double z, int dim,
                                                      bool strict, out long[] elementTags)
        {
            IntPtr elemTags, elemTags_n;
            elemTags = elemTags_n = IntPtr.Zero;
            var ierr = 0;
            elementTags = null;
            var strictInt = Convert.ToInt32(strict);
            gmshModelMeshGetElementsByCoordinates(x, y, z, ref elemTags, ref elemTags_n, dim, strictInt, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(elemTags, out elementTags, elemTags_n);
            return status;
        }
        /// <summary>
        /// Получить качество элемента
        /// </summary>
        /// <param name="elementTags">Идентификаторы элементов</param>
        /// <param name="qualityName">Тип квалификатора</param>
        /// <param name="task">Задача</param>
        /// <param name="numTasks">Число задач</param>
        /// <param name="elementsQuality">Массив значений качества</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshGetElementQualities(long[] elementTags, string qualityName,
                                                 long task, long numTasks, out double[] elementsQuality)
        {
            var eTags = elementTags.Select(x => (IntPtr)x).ToArray();
            var elemQuality = IntPtr.Zero;
            var elemQuality_n = IntPtr.Zero;
            var ierr = 0;
            elementsQuality = null;
            gmshModelMeshGetElementQualities(eTags, (IntPtr)elementTags.Length, ref elemQuality, ref elemQuality_n,
                                             qualityName, (IntPtr)task, (IntPtr)numTasks, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(elemQuality, out elementsQuality, elemQuality_n);
            return status;
        }
        #endregion
        #region "Mesh Field managed methods"
        /// <summary>
        /// Возвращает список объектов MeshField
        /// </summary>
        /// <param name="tags">Идентифкаторы объектов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshFieldList(out int[] tags)
        {
            IntPtr tagsInt = IntPtr.Zero;
            IntPtr tagsInt_n = IntPtr.Zero;
            var ierr = 0;
            tags = null;
            gmshModelMeshFieldList(ref tagsInt, ref tagsInt_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(tagsInt, out tags, tagsInt_n);
            return status;
        }
        /// <summary>
        /// Возвращает тип поля по заданному идентификатору
        /// </summary>
        /// <param name="tag">Идентификатор поля</param>
        /// <param name="fieldType">Тип поля</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshFieldGetType(int tag, out string fieldType)
        {
            IntPtr fType = IntPtr.Zero;
            var ierr = 0;
            fieldType = null;
            gmshModelMeshFieldGetType(tag, ref fType, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                fieldType = GetStringPtr(fType);
                gmshFree(fType);
            }

            return status;
        }
        /// <summary>
        /// Возвращает строку значений для опции
        /// </summary>
        /// <param name="tag">Идентификатор поля</param>
        /// <param name="option">Имя опции</param>
        /// <param name="value">Строка значений</param>
        /// <returns>Успешность вызова метода</returns>
        public bool ModelMeshFieldGetString(int tag, string option, out string value)
        {
            IntPtr valueInt = IntPtr.Zero;
            var ierr = 0;
            value = null;
            gmshModelMeshFieldGetString(tag, option, ref valueInt, ref ierr);
            var status = ierr == 0;
            if (status)
                value = GetStringPtr(valueInt);
            return status;
        }
        /// <summary>
        /// Возвращает список значений для опции
        /// </summary>
        /// <param name="tag">Идентификатор поля</param>
        /// <param name="option">Имя опции</param>
        /// <param name="values">Значения опции</param>
        /// <returns>Успешность вызова метода</returns>
        public bool MeshFieldGetNumbers(int tag, string option, out double[] values)
        {
            IntPtr valuesInt = IntPtr.Zero;
            IntPtr valuesInt_n = IntPtr.Zero;
            var ierr = 0;
            values = null;
            gmshModelMeshFieldGetNumbers(tag, option, ref valuesInt, ref valuesInt_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(valuesInt, out values, valuesInt_n);
            return status;
        }
        #endregion
        #region "Utility methods"
        /// <summary>
        /// Удалить геометрические сущности указанные парой dim:tags
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tags</param>
        /// <param name="recursive">Рекурсивно удалять детей</param>
        /// <returns>Успешность вызова метода</returns>
        public bool RemoveEntities(int[] dimTags, bool recursive = false)
        {
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                if (!RemoveAdjointEntities(dimTags[i - 1], dimTags[i]))
                    return false;
                if (!RemoveEntity(new int[] { dimTags[i - 1], dimTags[i] }, recursive))
                    return false;
            }
            return true;
        }

        private bool RemoveEntity(int[] dimTags, bool recursive)
        {
            var ierr = 0;
            var recFlag = Convert.ToInt32(recursive);
            string fileName;
            ModelGetFileName(out fileName);
            var ext = Path.GetExtension(fileName);
            if (geomExtensions.Contains(ext))
                gmshModelOccRemove(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
            else
                gmshModelRemoveEntities(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
            return ierr == 0;
        }

        private bool RemoveAdjointEntities(int dim, int tag)
        {
            int[] upward, downward;
            if (!ModelGetAdjacencies(dim, tag, out upward, out downward))
                return false;
            for (var i = 0; i < upward.Length; ++i)
            {
                if (!RemoveAdjointEntities(dim + 1, upward[i]))
                    return false;
            }
            if (!RemoveEntity(new int[] { dim, tag }, false))
                return false;
            return true;
        }
        /// <summary>
        /// Возвращает список кривых
        /// </summary>
        /// <param name="tags">Идентификаторы кривых</param>
        /// <param name="status">Успешность вызова метода</param>
        /// <returns>Список кривых</returns>
        public List<ILineObject> CreateLines(int[] tags, ref bool status)
        {
            var lines = new List<ILineObject>();
            double[] minBounds;
            double[] maxBounds;
            status = true;
            for (var i = 1; i < tags.Length; i += 2)
            {
                if (!ModelGetParametrizationBounds(1, tags[i], out minBounds, out maxBounds))
                {
                    status = false;
                    lines.Clear();
                    break;
                }
                var newPB = minBounds.Concat(maxBounds).ToArray();
                if (!IsLine(1, tags[i], newPB))
                {
                    var ierr = 0;
                    var curvePoints = 0.0;
                    gmshOptionGetNumber("Geometry.NumSubEdges", ref curvePoints, ref ierr);
                    newPB = SetCustomStepParametrization(newPB, (int)curvePoints);
                }
                double[] coords;
                ModelGetValue(1, tags[i], newPB, out coords);
                var points = ConvertLocalCoordsToPoints3D(coords);
                var nodes = new List<Node>();
                var num = 0;
                foreach (var pair in points.Bigrams(false))
                {
                    var line = new Node[] { new Node(num++, pair.Item1), new Node(num++, pair.Item2) };
                    nodes.AddRange(line);
                }

                lines.Add(new Line(tags[i], nodes.ToArray()));
            }
            return lines;
        }

        private double[] SetCustomStepParametrization(double[] paramsCoord, int steps)
        {
            var step = (paramsCoord[1] - paramsCoord[0]) / (steps - 1);
            Array.Resize(ref paramsCoord, steps);
            for (var i = 0; i < steps; ++i)
                paramsCoord[i] = paramsCoord[0] + step * i;
            return paramsCoord;
        }

        private bool IsLine(int dim, int tag, double[] paramsBound)
        {
            double[] derivative;
            ModelGetDerivative(dim, tag, paramsBound, out derivative);
            var derCount = derivative.Length / 3;
            for (var i = 1; i < derCount; ++i)
                for (var j = 0; j < 3; ++j)
                    if (Math.Abs(derivative[i * 3 + j] - derivative[(i - 1) * 3 + j]) > 1E-8)
                        return false;
            return true;
        }
        /// <summary>
        /// Возвращает список контрольных точек
        /// </summary>
        /// <param name="tags">Идентификаторы</param>
        /// <returns>Список контрольных точек</returns>
        public List<IModelObject> CreateControlPoints(int[] tags)
        {
            var nodes = new List<IModelObject>();
            double[] coords;
            for (var i = 1; i < tags.Length; i += 2)
            {
                if (!ModelGetValue(0, tags[i], new double[] { }, out coords))
                {
                    nodes.Clear();
                    break;
                }
                var points = ConvertLocalCoordsToPoints3D(coords);
                for (var j = 0; j < points.Length; ++j)
                    nodes.Add(new Node(tags[i], points[j], ObjKind.Вершина));
            }
            return nodes;
        }

        private Point3D[] ConvertLocalCoordsToPoints3D(double[] coords)
        {
            var points = new Point3D[coords.Length / 3];
            for (int i = 2, j = 0; i < coords.Length; i += 3, ++j)
            {
                var point = new Point3D((float)coords[i - 2], (float)coords[i - 1], (float)coords[i]);
                points[j] = point;
            }
            return points;
        }
        /// <summary>
        /// Возвращает список элементов сетки
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="status">Успешность вызова метода</param>
        /// <returns>Список элементов сетки</returns>
        public List<ISurfaceElement> GetMeshEntities(int dim, int tag, ref bool status)
        {
            ElementProperties[] properties;
            int[] elementTypes;
            long[][] elemTags, nodeTags;
            long[] nodesT;
            double[] coords, parametric;
            var list = new List<ISurfaceElement>();
            status = false;
            if (ModelMeshGetElements(dim, tag, out elementTypes, out elemTags, out nodeTags))
            {
                if (ModelMeshGetElementProperties(elementTypes, out properties))
                {
                    for (int i = 0; i < properties.Length; ++i)
                    {
                        var elements = elemTags[i];
                        ModelMeshGetNodesByElementType(elementTypes[i], -1, false, out nodesT, out coords, out parametric);
                        for (var j = 0; j < elements.Length; ++j)
                        {
                            var nodesCount = properties[i].numNodes;
                            var nodesPerElem = new Node[nodesCount];
                            for (int k = 0; k < nodesCount; ++k)
                            {
                                var coordStride = j * nodesCount * 3 + k * 3;
                                var tagStride = j * nodesCount + k;
                                var x = (float)coords[coordStride + 0];
                                var y = (float)coords[coordStride + 1];
                                var z = (float)coords[coordStride + 2];
                                var point = new Point3D(x, y, z);
                                nodesPerElem[k] = new Node((int)nodeTags[i][tagStride], point);
                            }
                            list.Add(CreateModelObject(ref properties[i].elementName, elements[j], nodesPerElem));
                        }
                    }
                    status = true;
                }
            }
            return list;
        }
        /// <summary>
        /// Возвращает список узлов сетки
        /// </summary>
        /// <param name="status">Успешность вызова метода</param>
        /// <returns>Список узлов сетки</returns>
        public List<ModelObject> GetNodes(ref bool status)
        {
            long[] nodeTags;
            double[] local, parametric;
            var nodes = new List<ModelObject>();
            if (ModelMeshGetNodes(-1, -1, false, false, out nodeTags, out local, out parametric))
            {
                for (var i = 0; i < nodeTags.Length; ++i)
                {
                    var point = new Point3D((float)local[i * 3], (float)local[i * 3 + 1], (float)local[i * 3 + 2]);
                    nodes.Add(new Node((int)nodeTags[i], point));
                }
                status = true;
            }
            return nodes;
        }
        /// <summary>
        /// Удалить элементы сетки
        /// </summary>
        /// <param name="elementTags">Идентификаторы элементов</param>
        /// <returns>Успешность вызова метода</returns>
        public bool DeleteMeshElements(long[] elementTags)
        {
            var ierr = 0;
            var status = false;
            gmshModelMeshSetVisibility(elementTags, (IntPtr)elementTags.Length, 0, ref ierr);
            status = ierr == 0;
            if (status)
            {
                gmshPluginSetNumber("Invisible", "DeleteElements", 1, ref ierr);
                status = ierr == 0;
                if (status)
                {
                    gmshPluginRun("Invisible", ref ierr);
                    status = ierr == 0;
                }
            }
            return status;
        }
        /// <summary>
        /// Вернуть записи в журнале
        /// </summary>
        /// <param name="logs">Массив записей</param>
        /// <returns>Успешность вызова метода</returns>
        public bool LoggerGet(out string[] logs)
        {
            var ierr = 0;
            IntPtr log = IntPtr.Zero;
            IntPtr log_n = IntPtr.Zero;
            gmshLoggerGet(ref log, ref log_n, ref ierr);
            logs = GetArrayOfString(log, log_n);
            return ierr == 0;
        }
        /// <summary>
        /// Вернуть последнюю запись об ошибке в журнале
        /// </summary>
        /// <param name="log">Запись об ошибке</param>
        /// <returns>Успешность вызова метода</returns>
        public bool LoggerGetLastError(out string log)
        {
            var ierr = 0;
            IntPtr error = IntPtr.Zero;
            gmshLoggerGetLastError(ref error, ref ierr);
            log = GetStringPtr(error);
            return ierr == 0;
        }

        private ISurfaceElement CreateModelObject(ref string elementName,
                                                  long elementId, Node[] nodes)
        {
            if (elementName.Contains("Triangle"))
                return new Triangle((int)elementId, nodes);
            else if (elementName.Contains("Quadrilateral"))
                return new Quad((int)elementId, nodes);
            else if (elementName.Contains("Tetrahedron"))
                return new Tetra((int)elementId, nodes);
            else if (elementName.Contains("Hexahedron"))
                return new Hexa((int)elementId, nodes);
            else
                return new Penta((int)elementId, nodes);
        }
        #endregion
        #region "Memory managed methods"
        private void FillTwoDimArray(IntPtr eTags, IntPtr eTags_n, IntPtr eTags_nn, out long[][] eData)
        {
            var objsTypesCount = eTags_nn.ToInt32();
            IntPtr[] tagsData, lengthData;
            NewTwoDimArray(eTags, eTags_n, eTags_nn, out tagsData, out lengthData, out eData);
            for (var i = 0; i < objsTypesCount; ++i)
                GetObjectsData(tagsData[i], out eData[i], lengthData[i]);
            gmshFree(eTags);
        }

        private void NewTwoDimArray(IntPtr eTags, IntPtr eTags_n, IntPtr eTags_nn,
                                    out IntPtr[] tagsData, out IntPtr[] lengthData, out long[][] eData)
        {
            var objectsCount = eTags_nn.ToInt32();
            eData = new long[objectsCount][];
            tagsData = new IntPtr[objectsCount];
            lengthData = new IntPtr[objectsCount];

            Marshal.Copy(eTags, tagsData, 0, objectsCount);
            Marshal.Copy(eTags_n, lengthData, 0, objectsCount);
            gmshFree(eTags_n);
        }

        private void GetObjectsData<T>(IntPtr src, out T[] dst, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            dst = new T[count];
            Marshal.Copy(src, buffer, 0, size);
            Buffer.BlockCopy(buffer, 0, dst, 0, size);
            gmshFree(src);
        }

        private string[] GetArrayOfString(IntPtr array, IntPtr length)
        {
            var arrayLength = length.ToInt32();
            var stringArray = new IntPtr[arrayLength];
            var result = new string[arrayLength];
            Marshal.Copy(array, stringArray, 0, arrayLength);
            for (var i = 0; i < arrayLength; ++i)
            {
                result[i] = Marshal.PtrToStringAnsi(stringArray[i]);
                gmshFree(stringArray[i]);
            }
            gmshFree(array);
            return result;
        }
        #endregion
    }
}
