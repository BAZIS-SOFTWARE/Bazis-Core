using Model.Interfaces;
using Project.Interfaces;
using Project.Interfaces.Tasks;
using Geometry;
using Model.GeometryObjects;
using Model.MeshObjects;
using System.Collections.Generic;
using System.Drawing;
using Model.Interfaces.ObjectsCollections;
using Model.Interfaces.MeshObjects;
using OperationalController.GmshController;
using PropertiesCalculator.MaterialData;
using PropertiesCalculator.FunctionData;
using System;
using System.Diagnostics;

namespace OperationalController
{
    /// <summary>
    /// IModelController
    /// </summary>
    public partial interface IController
    {
        /// <summary>
        /// FunctionsDB
        /// </summary>
        FunctionDBData FunctionsDB { get; set; }
        /// <summary>
        /// MaterialsDB
        /// </summary>
        MaterialDBData MaterialsDB { get; set; }
        /// <summary>
        /// ProjectKind
        /// </summary>
        TaskKind ProjectKind { get; set; }
        /// <summary>
        /// GetProjectType
        /// </summary>
        TaskType ProjectType { get; set; }
        /// <summary>
        /// TaskData
        /// </summary>
        ITaskData TaskData {get;}
        /// <summary>
        /// ModelData
        /// </summary>
        IModelData ModelData { get; }
        /// <summary>
        /// Path
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        void Load(string path);
        /// <summary>
        /// ExportMesh
        /// </summary>
        /// <param name="v"></param>
        void ExportMesh(string v);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="projectFile"></param>
        void CreateProject(string projectFile);

        /// <summary>
        /// GetModelObject
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        IModelObject GetModelObject(ObjType objType, int setNumber);
        /// <summary>
        /// GetModelObjects
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetModelObjects(ObjType objType);
        /// <summary>
        /// GetAllModelObjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelObject> GetAllModelObjects();
        /// <summary>
        /// GetModelGroup
        /// </summary>
        /// <returns>
        /// Возвращает группу объектов
        /// </returns>
        IGroup GetModelGroup(int grIndex);
        /// <summary>
        /// GetAllModelGroups
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGroup> GetAllModelGroups();
        /// <summary>
        /// GetAllCondData
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICondData> GetAllCondData();
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> GetModelElements();
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
        /// DeleteGroup
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbs"></param>
        void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs);
        /// <summary>
        /// DeleteCond
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool DeleteCond(int index);
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
        [Obsolete("По возможности не использовать")]
        IObjsPresenter CreateModelObjectsPresentor(ObjType objType);

        /// <summary>
        /// CreateModelObjectsPresentor.Предпочтительнее использовать его
        /// </summary>
        /// <param name="setInfo"></param>
        /// <returns></returns>
        IObjsPresenter CreateModelObjectsPresentor(ISetInfo setInfo);

        /// <summary>
        /// GetModelSetsInfo
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<ISetInfo> GetModelSetsInfo(ObjType objType);
        /// <summary>
        /// GetAllModelSetsInfo
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISetInfo> GetAllModelSetsInfo();
        /// <summary>
        /// GetModelVolumes
        /// </summary>
        /// <returns></returns>
        IEnumerable<VolumeFigure> GetModelVolumes();
        /// <summary>
        /// GetModelSetInfo
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        ISetInfo GetModelSetInfo(ObjType objType, string setName);
        /// <summary>
        /// GetModelSetInfo
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumber"></param>
        /// <returns></returns>
        ISetInfo GetModelSetInfo(ObjType objType, int objNumber);
        /// <summary>
        /// ChangeMeshSetName
        /// </summary>
        /// <param name="elemDim"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        void ChangeMeshSetName(int elemDim, string oldName, string newName);
        /// <summary>
        /// SetModelObjectsBackColor
        /// </summary>
        /// <param name="objType"></param>
        void SetModelObjectsBackColor(ObjType objType);
        /// <summary>
        /// ChangeInsideSurfacesState
        /// </summary>
        /// <param name="state"></param>
        void ChangeInsideSurfacesState(bool state);

        /// <summary>
        /// ChangeTaskType
        /// </summary>
        /// <param name="taskType"></param>
        void ChangeTaskType(TaskType taskType);

        /// <summary>
        /// ChangeTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        void ChangeTaskKind(TaskKind taskKind);
        /// <summary>
        /// ChangeMeshSetOrder
        /// </summary>
        /// <param name="elemDim"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        void ChangeMeshSetOrder(int elemDim, string name, int order);
        /// <summary>
        /// Import
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="gmshController"></param>
        void ImportCAD(string projectFile, IGmshController gmshController);
        /// <summary>
        /// GenerateMesh
        /// </summary>
        /// <param name="elemDim"></param>
        /// <param name="controller"></param>
        void GenerateMesh(int elemDim, IGmshController controller);


    }
}
