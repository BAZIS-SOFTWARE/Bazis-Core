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

    }
}
