using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;

namespace OperationalController.GmshController
{

    /// <summary>
    /// Интерфейс контроллера GmshApi
    /// </summary>
    public interface IGmshController
    {
        /// <summary>
        /// Основной класс враппера
        /// </summary>
        Gmsh Gmsh { get;}
        /// <summary>
        /// Загрузка gmsh.dll
        /// </summary>
        /// <param name="dllToLoad">Путь до DLL</param>
        void Load(string dllToLoad);
        /// <summary>
        /// Выгрузка gmsh.dll
        /// </summary>
        void Unload();
        /// <summary>
        /// Возвращает список отсортированных узлов сетки.
        /// Если запрашивается точки для dim <=0, флаг includeBoundary установить в false, иначе будут дубликаты
        /// </summary>
        /// <param name="dim">Размерность dim</param>
        /// <param name="tag">Идентификатор узла или (-1 для всех)</param>
        /// <param name="includeBoundary">Включать граничные точки</param>
        /// <returns>Список узлов сетки</returns>
        List<Node> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = true);
        /// <summary>
        /// Возвращает список контрольных точек
        /// </summary>
        /// <returns>Список контрольных точек</returns>
        List<GeometryPoint> CreateControlPoints();
        /// <summary>
        /// Возвращает список кривых
        /// </summary>
        /// <returns>Список кривых</returns>
        List<Curve> CreateLines();
        /// <summary>
        /// Возвращает список элементов сетки
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="nodes">Отсортированные узлы</param>
        /// <returns>Список элементов сетки</returns>
        List<IElement> GetMeshEntities(int dim, int tag, List<Node> nodes);
        /// <summary>
        ///GetMeshObjects
        /// </summary>
        /// <returns>
        /// item1 - nodes
        /// item2 - els1d
        /// item3 - els2d
        /// item4 - els3d
        /// </returns>
        (List<Node>, List<ILineObject<Node>>, List<Element2D>, List<Element3D>) GetMeshObjects();
        /// <summary>
        /// Возвращает длину кривой для заданного идентификатора
        /// </summary>
        /// <param name="curveTag">[In]Идентификатор кривой</param>
        /// <returns>Длина кривой</returns>
        double GetCurveLength(int curveTag);
        /// <summary>
        /// Удалить элементы сетки
        /// </summary>
        /// <param name="elementTags">Идентификаторы элементов</param>
        /// <returns>Успешность вызова метода</returns>
        void DeleteMeshElements(IntPtr[] elementTags);
        /// <summary>
        /// Построит геометрию по сетке автоматическим способом из загруженного сеточного формата
        /// </summary>
        /// <param name="surfAngle">[In]Пороговый угол разбиения поверхностей</param>
        /// <param name="curveAngle">[In]Пороговый угол разбиения кривых</param>
        void CreateGeometry(double surfAngle = Math.PI / 2, double curveAngle = Math.PI);
        /// <summary>
        /// Вытянуть триангулированные геметрические объемы, если они имеются
        /// </summary>
        /// <returns>Список VolumeFigure</returns>
        List<VolumeFigure> GetVolumeFigures();
        /// <summary>
        /// Вытянуть триангулированные геметрические поверхности, если они имеются
        /// </summary>
        /// <returns>Список SurfaceFigure</returns>
        List<SurfaceFigure> GetSurfaceFigures();
    }
}
