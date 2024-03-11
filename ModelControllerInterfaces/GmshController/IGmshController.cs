using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.GmshController
{
    public interface IGmshController
    {
        /// <summary>
        /// Загрузка gmsh.dll
        /// </summary>
        /// <param name="dllToLoad">Путь до DLL</param>
        void Load(string dllToLoad);

        /// <summary>
        /// Открывает скрипт и ассоциирует данные с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void Merge(string fileName, ref int ierr);///

        /// <summary>
        /// Ассоциирует имя файла с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelSetFileName(string fileName, ref int ierr);                                                                           
        /// <summary>                                                                                   
        /// Удаляет текущую модель из списка моделей                                                                             
        /// </summary>                                                                         
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelRemove(ref int ierr);
        /// <summary>
        /// Возвращает путь до файла ассоциированный с данной моделью
        /// </summary>
        /// <param name="fileName">Путь до файла ассоциированный с данной моделью</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetFileName(out string fileName);
        /// <summary>
        /// Устанавливает имя для текущей модели
        /// </summary>
        /// <param name="name">[In]Имя для текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelSetCurrent(string name, ref int ierr);
        /// <summary>
        /// Добавляет новую модель с заданным именем в список моделей и делает ее текущей
        /// </summary>
        /// <param name="name">[In]Имя новой модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelAdd(string name, ref int ierr);
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <param name="current">Имя текущей модели</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetCurrent(out string current);
        /// <summary>
        /// Возвращает список моделей в gmsh API
        /// </summary>
        /// <param name="modelList">Массив строк имен моделей</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelList(out string[] modelList);
        /// <summary>
        /// Вернуть последнюю запись об ошибке в журнале
        /// </summary>
        /// <param name="log">Запись об ошибке</param>
        /// <returns>Успешность вызова метода</returns>
        bool LoggerGetLastError(out string log);
        /// <summary>
        /// Возвращает список кривых
        /// </summary>
        /// <param name="tags">Идентификаторы кривых</param>
        /// <returns>Список кривых</returns>
        List<ILine> CreateLines(int[] tags);
        /// <summary>
        /// Возвращает список контрольных точек
        /// </summary>
        /// <param name="tags">Идентификаторы</param>
        /// <returns>Список контрольных точек</returns>
        List<IGeometryPoint> CreateControlPoints(int[] tags);
        /// <summary>
        /// Возвращает список элементов сетки
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="nodes">Отсортированные узлы</param>
        /// <returns>Список элементов сетки</returns>
        List<ISurfaceElement> GetMeshEntities(int dim, int tag, List<INode> nodes);
        /// <summary>
        /// Возвращает список отсортированных узлов сетки.
        /// Если запрашивается точки для dim <=0, флаг includeBoundary установить в false, иначе будут дубликаты
        /// </summary>
        /// <param name="dim">Размерность dim</param>
        /// <param name="tag">Идентификатор узла или (-1 для всех)</param>
        /// <param name="includeBoundary">Включать граничные точки</param>
        /// <returns>Список узлов сетки</returns>
        List<INode> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = true);
        /// <summary>
        ///GetMeshObjects
        /// </summary>
        /// <returns>
        /// item1 - nodes
        /// item2 - els1d
        /// item3 - els2d
        /// item4 - els3d
        /// </returns>
        (List<INode>, List<ILineObject<INode>>, List<IElement2D>, List<IElement3D>) GetMeshObjects();
        /// <summary>
        /// Возвращает список всех пар размерность:идентификатор для заданной размерности
        /// </summary>
        /// <param name="dimTags">Пары dim:tag</param>
        /// <param name="dim">Заданная размерность</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetGeometryEntities(out int[] dimTags, int dim);
        /// <summary>
        /// Возвращает максимальную размерность текущего геометрического объекта
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Максимальная размерность</returns>
        int GetGeometryObjectDimension(ref int ierr);
        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void OptionSetNumber(string name, double value, ref int ierr);

        /// <summary>
        /// Очищает список моделей
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void Clear(ref int ierr);

        /// <summary>
        /// Открывает скрипт файл .geo, .stp, .igs создает модель и делает ее текущей
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void Open(string fileName, ref int ierr);
        /// <summary>
        /// Удаляет gmsh геометрически сущности заданные парой dimTags
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="recursive">[In]Рекурсивно улалять граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelRemoveEntities(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);

        /// <summary>
        /// Удалить геометрические сущности указанные парой dim:tags
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tags</param>
        /// <param name="delChild">Рекурсивно удалять детей</param>
        /// <returns>Успешность вызова метода</returns>
        bool RemoveGeometryObjects(int[] dimTags, bool delChild = false);

        /// <summary>
        /// Задает имя сущности для текущих dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelSetEntityName(int dim, int tag, string name, ref int ierr);

        /// <summary>
        /// Возвращает имя сущности для указанной dim:tag
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="name">Имя</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetGeometryEntityName(int dim, int tag, out string name);


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
        void ModelGetBoundingBox(int dim, int tag, ref double xmin,
                                                      ref double ymin, ref double zmin,
                                                      ref double xmax, ref double ymax,
                                                      ref double zmax, ref int ierr);

        /// <summary>
        /// Возвращает соседей верхнего\нижнего уровня для текущей пары dim : tag
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="upward">Соседи верхнего уровня</param>
        /// <param name="downward">Соседи нижнего уровня</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetAdjacencies(int dim, int tag, out int[] upward, out int[] downward);
        /// <summary>
        /// Возвращает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <param name="coord">Локальные координаты</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetValue(int dim, int tag, double[] parametricCoord, out double[] coord);

        /// <summary>
        /// Возвращает минимальные максимальные границы параметризации
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="min">Минимальные границы</param>
        /// <param name="max">Максимальные границы</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetParametrizationBounds(int dim, int tag, out double[] min, out double[] max);

        /// <summary>
        /// Определяет находится ли точка параметрическая или локальная внутри массива коодинат
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coord">[In]Масив координат</param>
        /// <param name="coord_n">[In]Количество координат</param>
        /// <param name="parametric">[In]Флаг являяется ли координата параметрической</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Число координат лежащих внутри модели</returns>
        int ModelIsInside(int dim, int tag, double[] coord, IntPtr coord_n,
                                                 int parametric, ref int ierr);


        /// <summary>
        /// Задает количество опорных точек на кривой, по которой будет строиться сетка
        /// </summary>
        /// <param name="tag">[In]Идентификатор кривой</param>
        /// <param name="numNodes">[In]Число точек на кривой</param>
        /// <param name="meshType">[In]Алгоритм распределения точек</param>
        /// <param name="coef">[In]Коэффициент прогрессии</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelMeshSetTransfiniteCurve(int tag, int numNodes, string meshType,
                                                                  double coef, ref int ierr);

        /// <summary>
        /// Связать имя аттрибута со значениями
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="values">[In]Массив значений</param>
        /// <param name="values_n">[In]Размер массива значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelSetAttribute(string name, string[] values, IntPtr values_n, ref int ierr);

        /// <summary>
        /// Возвратит значения для заданного аттрибута
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        /// <param name="attributes">Значения аттрибута</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetAttribute(string name, out string[] attributes);

        /// <summary>
        /// Возвращает список имен аттрибутов
        /// </summary>
        /// <param name="names">Список имен аттрибутов</param>
        /// <returns>Успешность вызова метода</returns>
        bool ModelGetAttributeNames(out string[] names);
        /// <summary>
        /// Удалить имя аттрибута
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelRemoveAttribute(string name, ref int ierr);
        /// <summary>
        /// Генерирует сетку в текущей размерности
        /// </summary>
        /// <param name="dim">[In]Размерность в которой нужно сгенерировать модель</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
       void ModelMeshGenerate(int dim, ref int ierr);


        /// <summary>
        /// Добавляет идентификаторы поверхностей, формирующих замкнутую оболочку, возвращает идентификатор оболочки
        /// </summary>
        /// <param name="surfaceTags">[In]Массив идентификаторов поверхностей</param>
        /// <param name="surfaceTags_n">[In]Длина массива идентификаторов</param>
        /// <param name="tag">[In]Идентификатор оболочки или -1 для автоматического назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Идентификатор оболочки</returns>
        int ModelGeoAddSurfaceLoop(int[] surfaceTags, IntPtr surfaceTags_n, int tag, ref int ierr);

        /// <summary>
        /// Формирует объем из заданных оболочек, первый элемент массива замкнутая оболочка, следующие элементы
        /// указывают отверстия в геометрии
        /// </summary>
        /// <param name="shellTags">[In]Массив идентификаторов оболочек</param>
        /// <param name="shellTags_n">[In]Длина массива идентификаторов</param>
        /// <param name="tag">[In]Идентификатор объема или -1 для автоматического назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Идентификатор объема</returns>
        int ModelGeoAddVolume(int[] shellTags, IntPtr shellTags_n, int tag, ref int ierr);

        /// <summary>
        /// Проводит синхронизацию с ядром gmsh (например после добавления геометрических сущностей)
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelGeoSynchronize(ref int ierr);

        /// <summary>
        /// Получить плотности для геометрии
        /// </summary>
        /// <param name="dimTags">[In]Массив dim:tags</param>
        /// <returns>Геометрические плотности</returns>
        double[] GetMeshDensity(int[] dimTags);

        /// <summary>
        /// Задает плотность(MeshSize) для конкретных сущностей(dim:tags)
        /// </summary>
        /// <param name="dimTags">[In]Масив dim:tags</param>
        /// <param name="dimTags_n">[In]Размер массива dim:tags</param>
        /// <param name="size">[In]Задаваемая плотность</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelMeshSetSize(int[] dimTags, IntPtr dimTags_n, double size, ref int ierr);
        /// <summary>
        /// Удаляет сущность заданную именем
        /// </summary>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        void ModelRemoveEntityName(string name, ref int ierr);
    }
}
