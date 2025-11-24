using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using Model.ObjectsCollections;
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
        /// SetGradientVolume
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="power"></param>
        /// <param name="distMax"></param>
        /// <param name="surfSize"></param>
        /// <param name="coreSize"></param>
        void SetGradientVolume(int tag, double power, double distMax, double surfSize, double coreSize);
        /// <summary>
        /// SetTransfiniteVolume
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cornerTags"></param>
        void SetTransfiniteVolume(int tag, int[] cornerTags = null);
        /// <summary>
        /// GetTransfiniteVolume
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string[] GetTransfiniteVolume(int tag);
        /// <summary>
        /// GetSurfaceNodes
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int[] GetSurfaceNodes(int tag);
        /// <summary>
        /// SetRecombineSurface
        /// </summary>
        /// <param name="tag"></param>
        void SetRecombineSurface(int tag);
        /// <summary>
        /// GetRecombineSurface
        /// </summary>
        /// <param name="tag"></param>
        bool GetRecombineSurface(int tag);
        /// <summary>
        /// SetTransfiniteSurface
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="arrangement"></param>
        /// <param name="cornerTags"></param>
        void SetTransfiniteSurface(int tag, Arrangement arrangement = Arrangement.Left,int[] cornerTags = null);
        /// <summary>
        /// SetTransfiniteCurve
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="numNodes"></param>
        /// <param name="meshType"></param>
        /// <param name="coef"></param>
        void SetTransfiniteCurve(int tag, int numNodes, MeshType meshType = MeshType.Progression, double coef = 1.0);
        /// <summary>
        /// GetTransfiniteCurve
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string[] GetTransfiniteCurve(int tag);
        /// <summary>
        /// GetTransfiniteSurface
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string[] GetTransfiniteSurface(int tag);
        /// <summary>
        /// GetSize
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>Размер элемента</returns>
        double GetSize(int tag);
        /// <summary>
        /// SetSize
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="meshDize"></param>
        void SetSize(int tag, double meshDize);
        /// <summary>
        /// Возвращает список отсортированных узлов сетки принадлежащих геометрической сущности.
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
        /// GetMeshSets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<ObjectsSet<T>> GetMeshSets<T>(List<Node> nodes)
                   where T : class, IElement;
       
        /// <summary>
        /// Возвращает список элементов сетки принадлежащих геометрической сущности
        /// </summary>
        /// <param name="dim">Размерность</param>
        /// <param name="tag">Идентификатор (номер) сущности</param>
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
        double CalcCurveLength(int curveTag);

        /// <summary>
        /// Возвращает величину объема для заданного идентификатора
        /// </summary>
        /// <param name="volTag"></param>
        /// <returns></returns>
        double CalcVolume(int volTag);

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
        /// Вытянуть триангулированные геометрические объемы, если они имеются
        /// </summary>
        /// <returns>Список VolumeFigure</returns>
        List<VolumeFigure> CreateVolumeFigures();
        /// <summary>
        /// Вытянуть триангулированные геометрические поверхности, если они имеются
        /// </summary>
        /// <returns>Список SurfaceFigure</returns>
        List<SurfaceFigure> CreateSurfaceFigures();
        /// <summary>
        /// Возвращает поверхности рассортированные по наборам.
        /// Каждый набор представляет конкретный объем. 
        /// Если объемов нет, то остается набор "FreeSurfaces"
        /// </summary>
        /// <returns></returns>
        Tuple<IObjectsSet<SurfaceFigure>, VolumeData> CreateSurfaceData();
    }
}
