using Geometry;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Extrude3DForm2D
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="layers"></param>
        /// <param name="elements"></param>
        void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements);
        /// <summary>
        /// CreateElementsSet
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="dim"></param>
        void CreateElementsSet(string setName, int dim);

        /// <summary>
        /// CreateElement
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="dim"></param>
        /// <param name="nodeNumbers"></param>
        void CreateElement(string setName, int dim, int[] nodeNumbers);
        /// <summary>
        /// CreateNode
        /// </summary>
        /// <param name="number"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void CreateNode(int number, float x, float y, float z);
        /// <summary>
        /// CreateGroup
        /// </summary>
        /// <param name="grName"></param>
        /// <param name="objs"></param>
        void CreateGroup(string grName, IEnumerable<IModelObject> objs);
        /// <summary>
        /// CreateGroup
        /// </summary>
        /// <param name="objs"></param>
        void CreateGroup(IEnumerable<IModelObject> objs);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        ICondData Create(DataKind kind, string info);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="projectFile"></param>
        void CreateProject(string projectFile);

        /// <summary>
        /// CreateTask
        /// </summary>
        void CreateTask();
        /// <summary>
        /// CreateTask
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="functions"></param>
        void CreateTask(MaterialDBData materials, FunctionDBData functions);
        /// <summary>
        /// Create1DFrom2D
        /// </summary>
        /// <returns></returns>
        bool Create1DFrom2D();
        /// <summary>
        /// Create2DForm3D
        /// </summary>
        /// <returns></returns>
        bool Create2DForm3D();
        /// <summary>
        /// CreateBoundaryEdges
        /// </summary>
        /// <param name="linesNodes"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes);

        /// <summary>
        /// CreateQuadMeshOnPoligon
        /// </summary>
        /// <param name="points"></param>
        /// <param name="numberOfElem"></param>
        /// <returns></returns>
        void CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem);

        /// <summary>
        /// Создаёт фаски на нескольких кривых за одну операцию.
        /// </summary>
        /// <param name="curveTags">Теги кривых.</param>
        /// <param name="firstLength">Размер фаски на первой соседней поверхности.</param>
        /// <param name="secondLengthOrAngle">Размер на второй поверхности либо угол фаски в градусах.</param>
        /// <param name="isAngle"><c>true</c>, если <paramref name="secondLengthOrAngle"/> задаёт угол.</param>
        /// <param name="reflected"><c>true</c>, чтобы использовать вторую соседнюю поверхность как опорную.</param>
        /// <returns>Массив пар "размерность-тег" созданных геометрических сущностей.</returns>
        int[] CreateChamfer(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected);

    }
}
