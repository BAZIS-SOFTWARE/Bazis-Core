using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsUtility;
using OperationalController;
using OperationalController.SurfaceMeshCreator;
using OperationalController.MeshObjsUtility;
using OperationalController.ObjsCreator;
using Model.Interfaces;
using Project;
using Project.Interfaces.Tasks;
using Project.Interfaces;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using Geometry;
using Model.GeometryObjects;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System;
using OperationalController.Extensions;
using Model.Interfaces.ObjectsCollections;
using Model;

namespace OperationalController
{
    /// <summary>
    /// ProjectController
    /// </summary>
    public partial class Controller : IController
    {
        ProjectData project;

/// <inheritdoc/>

        public void Load()
        {
            if (project == null)
                throw new Exception("Проект не создан");
            project.Load();
        }
/// <inheritdoc/>

        public void Save()
        {
            if (project == null)
                throw new Exception("Проект не создан");
            project.Save();
        }
/// <inheritdoc/>

        public void Create(string projectFile)
        {
            project = new ProjectData();
            GeneralData.Name = System.IO.Path.GetFileName(projectFile);
            GeneralData.Path = System.IO.Path.GetDirectoryName(projectFile);
        }
/// <inheritdoc/>

        public void Create(string path, string proj)
        {
            project = new ProjectData(path, proj);
        }

        /// <inheritdoc/>

        public void ChangeTaskType(TaskType taskType)
        {
            GeneralData.TaskType = taskType;
            TaskData.Clear();
        }
      

/// <inheritdoc/>

        IBoundaryEdgesFinder boundaryEdgesFinder { get; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes)
        {
            return boundaryEdgesFinder.CreateBoundaryEdges(linesNodes, ModelData);
        }
        /// <inheritdoc/>
        public List<string> FindBoundaryEdges()
        {
            return boundaryEdgesFinder.Find(ModelData.ObjectData.GetAllElements().
                Select(x => (ISurfaceElement)x));
            
        }

        ISelectionHelper selectionHelper { get; } = new SelectionHelper();
/// <inheritdoc/>

        public List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color)
        {
            return selectionHelper.SelectNodeInDirection(ModelData.ObjectData, angleInGradus, numbFirst, numbSecond, color);
        }
        /// <inheritdoc/>
        public void SelectNodeInPlane(Geometry.Plane plane, Color color)
        {
            selectionHelper.SelectNodeInPlane(ModelData.ObjectData, plane, color);
        }
        /// <inheritdoc/>
        public List<int> SelectE2DInPlane(float angleInGradus, int startNumber, Color color)
        {
            return selectionHelper.SelectE2DInPlane(ModelData.ObjectData,angleInGradus, startNumber, color);
        }
/// <inheritdoc/>

        IFreeNodesFinder freeNodesFinder { get; } = new FreeNodesFinder();
        /// <inheritdoc/>
        public IEnumerable<int> FindFreeNodes()
        {
            return freeNodesFinder.Find(ModelData.ObjectData);
        }

/// <inheritdoc/>

        ICrossSectionMaker crossSectionMaker { get; } = new CrossSection();
/// <inheritdoc/>

        public SurfaceFigure GetSectionSurfaces(Geometry.Plane plane)
        {
            return crossSectionMaker.GetSectionSurfaces(ModelData.ObjectData.E3DCollection.GetObjects(), plane);
        }

        ICoincidentObjectsFinder coincidentObjectsFinder { get; } = new FindCoincidentObjects();
        /// <inheritdoc/>
        public List<List<int>> FindCoincidentObjects(ObjType objType, float distance)
        {
            var objs = ModelData.ObjectData.GetObjects(objType).ToList();
            return coincidentObjectsFinder.Find(objs, distance);
        }

        IObjectsMerger objectsMerger { get; } = new MergeObjects();

        /// <inheritdoc/>
        public void MergeNodes(List<List<int>> numbers)
        {
            objectsMerger.Merge(numbers, ModelData.ObjectData.NodesSet);
        }

        ISurfaceMeshCreator surfaceMeshCreator { get; set; }

/// <inheritdoc/>

        IObjectsRenumber objectsRenumber { get; } = new ObjectsRenumber();
/// <inheritdoc/>

        INormalCalculator normalCalculator { get; } = new NormalsCalculator();
        /// <inheritdoc/>
        public Dictionary<int, Point3D> CalcNodeNormals()
        {
            return normalCalculator.CalcNodeNormals(ModelData.ObjectData.E3DCollection.GetObjects());
        }
        /// <inheritdoc/>
        public IEnumerable<Curve> CalcElemsNormals(int elemDim)
        {
            if (elemDim > 3 | elemDim < 2)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 2)
                return normalCalculator.CalcElemsNormals(ModelData.ObjectData.E2DCollection.GetObjects());
            else
                return normalCalculator.CalcElemsNormals(ModelData.ObjectData.E3DCollection.GetObjects());

        }
        /// <inheritdoc/>

        IVolumeMeshCreator volumeMeshCreator { get; set; }

/// <inheritdoc/>

        IExtractor1DFrom2D extractor1DFrom2D { get; } = new Extract1DFrom2D();
/// <inheritdoc/>

        public void Create1DFrom2D(string setName)
        {
            var number = ModelData.ObjectData.E1DCollection.GetMaxNumber() + 1;
            var e1d = extractor1DFrom2D.Create(number, ModelData.ObjectData.E2DCollection.GetObjects().ToArray());
            ModelData.ObjectData.E1DCollection.AddRange("", e1d);
        }
        
/// <inheritdoc/>

        public void SetModelObjectsBackColor(ObjType objType)
        {
            ModelData?.ObjectData?.SetBackColor(objType);
        }



        /// <inheritdoc/>
        public ITaskData TaskData => project?.TaskData;
/// <inheritdoc/>

        public IModelData ModelData => project?.ModelData;
        /// <inheritdoc/>
        public IGeneralData GeneralData => project?.GeneralData;
    }
}
