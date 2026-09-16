using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Возвращает перечисление геометрических объектов, смежных с заданным объектом <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetAdjacentGeometryObjects(IModelObject obj, int direction);
        /// <summary>
        /// Находит объект модели заданного типа и номера.
        /// </summary>
        /// <param name="objType">Тип объекта.</param>
        /// <param name="setNumber">Номер (tag) объекта.</param>
        /// <returns>Найденный объект, либо <c>null</c>, если не найден.</returns>
        IModelObject GetModelObject(ObjType objType, int setNumber);
        /// <summary>
        /// Возвращает коллекцию объектов заданного типа в модели.
        /// </summary>
        /// <param name="objType">Тип объектов (<see cref="ObjType"/>).</param>
        /// <returns>Перечисление объектов, реализующих <see cref="IModelObject"/>.</returns>
        IEnumerable<IModelObject> GetModelObjects(ObjType objType);
        /// <summary>
        /// Возвращает все объекты модели всех типов.
        /// </summary>
        /// <returns>Перечисление всех объектов, реализующих <see cref="IModelObject"/>.</returns>
        IEnumerable<IModelObject> GetAllModelObjects();
        /// <summary>
        /// Возвращает все группы модели.
        /// </summary>
        /// <returns>Перечисление групп, реализующих <see cref="IGroup"/>.</returns>
        IEnumerable<IGroup> GetAllModelGroups();
        /// <summary>
        /// Возвращает все данные граничных/условий задачи.
        /// </summary>
        /// <returns>Перечисление объектов <see cref="ICondData"/>.</returns>
        IEnumerable<ICondData> GetAllCondData();
        /// <summary>
        /// Возвращает все узлы модели.
        /// </summary>
        /// <returns>Перечисление всех узлов модели в виде объектов <see cref="Node"/>.</returns>
        IEnumerable<Node> GetAllModelNodes();
        /// <summary>
        /// Возвращает все элементы (mesh elements) модели.
        /// </summary>
        /// <returns>Перечисление всех объектов <see cref="IElement"/>.</returns>
        IEnumerable<IElement> GetAllModelElements();
        /// <summary>
        /// Возвращает группу модели по индексу.
        /// </summary>
        /// <param name="grIndex">Индекс группы в коллекции.</param>
        /// <returns>Объект <see cref="IGroup"/>.</returns>
        IGroup GetModelGroup(int grIndex);
        /// <summary>
        /// Возвращает группу модели по имени.
        /// </summary>
        /// <param name="grName">Имя группы.</param>
        /// <returns>Объект группы <see cref="IGroup"/>, либо <c>null</c>, если группа не найдена.</returns>
        IGroup GetModelGroup(string grName);
        /// <summary>
        /// Возвращает данные условия по индексу.
        /// </summary>
        /// <param name="condIndex">Индекс условия в коллекции <see cref="TaskData"/>.</param>
        /// <returns>Объект <see cref="ICondData"/> соответствующий индексу.</returns>
        /// <exception cref="IndexOutOfRangeException">Если индекс выходит за пределы коллекции.</exception>
        ICondData GetCondData(int condIndex);
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        IEnumerable<IElement> GetModelElements(int dim, string setName);
        /// <summary>
        /// GetModelSurfaceElements
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        IEnumerable<ISurfaceElement> GetModelSurfaceElements(int dim);
        /// <summary>
        /// Возвращает поверхность модели по её номеру (tag).
        /// </summary>
        /// <param name="objNumb">Номер (tag) поверхности в модели.</param>
        /// <returns>Объект <see cref="SurfaceFigure"/> с указанным номером.</returns>
        SurfaceFigure GetModelSurface(int objNumb);
        /// <summary>
        /// Возвращает перечисление всех объёмных фигур (volume) модели.
        /// </summary>
        /// <returns>Последовательность объектов типа <see cref="VolumeFigure"/>.</returns>
        IEnumerable<VolumeFigure> GetModelVolumes();
        /// <summary>
        /// Возвращает кривую модели по её номеру (tag).
        /// </summary>
        /// <param name="objNumb">Номер (tag) кривой в модели.</param>
        /// <returns>Объект <see cref="Curve"/> с указанным номером.</returns>
        Curve GetModelCurve(int objNumb);
        /// <summary>
        /// Возвращает точку модели по её номеру (tag).
        /// </summary>
        /// <param name="objNumb">Номер (tag) точки в модели.</param>
        /// <returns>Объект <see cref="GeometryPoint"/> с указанным номером.</returns>
        GeometryPoint GetModelPoint(int objNumb);

        /// <summary>
        /// Возвращает координаты точек геометрического объекта в параметрических координатах.
        /// Делегирует вызов в Gmsh API.
        /// </summary>
        /// <param name="dim">Размерность геометрического объекта.</param>
        /// <param name="tag">Тег (номер) геометрического объекта.</param>
        /// <param name="parametricCoord">Параметрические координаты (входной массив).</param>
        /// <returns>Массив координат точек в пространстве (обычно длина 3).</returns>
        double[] GetGeoObjPoints(int dim, int tag, double[] parametricCoord);

        /// <summary>
        /// Возвращает границы параметризации геометрического объекта.
        /// Делегирует вызов в Gmsh API.
        /// </summary>
        /// <param name="dim">Размерность объекта.</param>
        /// <param name="tag">Тег (номер) объекта.</param>
        /// <returns>Кортеж двух массивов: (minBounds[], maxBounds[]).</returns>
        Tuple<double[], double[]> GetParametrizationBounds(int dim, int tag);

        /// <summary>
        /// Возвращает строковое представление типа геометрического объекта в модели.
        /// Делегирует вызов в Gmsh API.
        /// </summary>
        /// <param name="dim">Размерность объекта.</param>
        /// <param name="tag">Тег (номер) объекта.</param>
        /// <returns>Тип объекта в виде строки.</returns>
        string GetType(int dim, int tag);

        /// <summary>
        /// Возвращает центр масс для указанного геометрического объекта (через OCC API Gmsh).
        /// </summary>
        /// <param name="dim">Размерность объекта.</param>
        /// <param name="tag">Тег (номер) объекта.</param>
        /// <returns>Тройка координат центра масс (x, y, z).</returns>
        (double X, double Y, double Z) GetCenterOfMass(int dim, int tag);

        /// <summary>
        /// Возвращает путь к загруженной геометрической модели.
        /// </summary>
        /// <returns>Путь к файлу геометрической модели.</returns>
        string GetGeometryFilePath();

        /// <summary>
        /// Возвращает границу (boundary) для набора топологических сущностей.
        /// Делегирует вызов в Gmsh API.
        /// </summary>
        /// <param name="dimTags">Массив пар (dim, tag) заданных сущностей — представляется как последовательность целых, ожидаемых Gmsh.</param>
        /// <param name="combined">Если <c>true</c>, объединять элементы в комбинированную границу.</param>
        /// <param name="oriented">Если <c>true</c>, возвращать ориентированную границу.</param>
        /// <param name="recursive">Если <c>true</c>, обрабатывать границу рекурсивно.</param>
        /// <returns>Массив номеров сущностей, составляющих границу.</returns>
        int[] GetBoundary(int[] dimTags, bool combined = true, bool oriented = true, bool recursive = false);

        /// <summary>
        /// Возвращает длину кривой по её тегу.
        /// </summary>
        /// <param name="curveTag">Тег (номер) кривой в модели.</param>
        /// <returns>Длина кривой в единицах модели (double). При ошибке реализация может выбросить исключение.</returns>
        double GetCurveLength(int curveTag);
    }
}
