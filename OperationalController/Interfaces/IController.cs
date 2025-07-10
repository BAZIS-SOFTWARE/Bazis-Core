using Model.Interfaces;
using Project.Interfaces;
using Project.Interfaces.Tasks;
using Geometry;
using Model.GeometryObjects;
using Model.MeshObjects;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// IModelController
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// TaskData
        /// </summary>
        ITaskData TaskData {get;}
        /// <summary>
        /// ModelData
        /// </summary>
        IModelData ModelData { get; }
        /// <summary>
        /// TaskData
        /// </summary>
        IGeneralData GeneralData { get; }
        /// <summary>
        /// Save
        /// </summary>
        void Save();
        /// <summary>
        /// Load
        /// </summary>
        void Load();
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="projectFile"></param>
        void Create(string projectFile);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="path"></param>
        /// <param name="proj"></param>
        void Create(string path, string proj);
        /// <summary>
        /// FindBoundaryEdges
        /// </summary>
        /// <returns></returns>
        List<string> FindBoundaryEdges();

        /// <summary>
        /// FindInterfacedNodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="elemDim"></param>
        /// <returns></returns>
        Dictionary<int, List<int>> FindInterfacedNodes(List<int> nodes, int elemDim);
        /// <summary>
        /// FindInterfacedNodes
        /// </summary>
        /// <param name="elemDim"></param>
        /// <returns></returns>
        Dictionary<int, List<int>> FindInterfacedNodes(int elemDim);
        /// <summary>
        /// Create2DForm3D
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        void Create2DForm3D(string setName);
        /// <summary>
        /// CreateBoundaryEdges
        /// </summary>
        /// <param name="linesNodes"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes);
        /// <summary>
        /// SelectNodeInDirection
        /// </summary>
        /// <param name="angleInGradus"></param>
        /// <param name="numbFirst"></param>
        /// <param name="numbSecond"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color);
        /// <summary>
        /// SelectNodeInPlane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="color"></param>
        void SelectNodeInPlane(Geometry.Plane plane, Color color);
        /// <summary>
        /// SelectE2DInPlane
        /// </summary>
        /// <param name="angleInGradus"></param>
        /// <param name="startNumber"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        List<int> SelectE2DInPlane(float angleInGradus, int startNumber, Color color);

        /// <summary>
        /// FindFreeNodes
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> FindFreeNodes();
        /// <summary>
        /// GetSectionSurfaces
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        SurfaceFigure GetSectionSurfaces(Geometry.Plane plane);
        /// <summary>
        /// FindCoincidentObjects
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        List<List<int>> FindCoincidentObjects(ObjType objType, float distance);
        /// <summary>
        /// MergeNodes
        /// </summary>
        /// <param name="numbers"></param>
        void MergeNodes(List<List<int>> numbers);
        /// <summary>
        /// CalcNodeNormals
        /// </summary>
        /// <returns></returns>
        Dictionary<int, Point3D> CalcNodeNormals();
        /// <summary>
        /// CalcElemsNormals
        /// </summary>
        /// <param name="elemDim"></param>
        /// <returns></returns>
        IEnumerable<Curve> CalcElemsNormals(int elemDim);
        /// <summary>
        /// Create1DFrom2D
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        void Create1DFrom2D(string setName);
        /// <summary>
        /// ChangeTaskType
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        void ChangeTaskType(TaskType taskType);
        /// <summary>
        /// DeleteGroup
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbs"></param>
        void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs);
        /// <summary>
        /// DeleteMeshGroup
        /// </summary>
        /// <param name="grName"></param>
        void DeleteModelGroup(string grName);
        /// <summary>
        /// DeleteMeshSet
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="name"></param>
        void DeleteModelSet(ObjType objType, string name);

        /// <summary>
        /// ClearMeshCollection
        /// </summary>
        /// <param name="objType"></param>
        void ClearModelCollection(ObjType objType);
        /// <summary>
        /// CreateModelObjectsPresentor
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IObjsPresenter CreateModelObjectsPresentor(ObjType objType);
        /// <summary>
        /// ChangeInsideSurfacesState
        /// </summary>
        /// <param name="state"></param>
        void ChangeInsideSurfacesState(bool state);
    }
}
