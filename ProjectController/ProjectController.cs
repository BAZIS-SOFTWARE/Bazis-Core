using ProjectController.MeshObjsCreator;
using ProjectController.MeshObjsUtility;
using ProjectController.ModelScenePresentator;
using ModelControllerInterfaces;
using ModelControllerInterfaces.SurfaceMeshCreator;
using ModelControllerInterfaces.MeshObjsUtility;
using ModelControllerInterfaces.Utilities;
using ModelControllerInterfaces.ObjsCreator;
using Model.Interfaces;
using Project;
using Project.Tasks;
using Model;
using Project.Interfaces.Tasks;
using Project.Interfaces;
using Model.MeshObjects;
using System.Collections.Generic;
using System;
using GmshApi;
using System.Reflection;
using System.Linq;
using Model.Interfaces.MeshObjects;
using System.Drawing;
using Geometry;

namespace ProjectController
{
    /// <summary>
    /// ModelController
    /// </summary>
    public class ProjectController : IProjectController
    {
        ProjectData project;
/// <inheritdoc/>


        public void Load(string projectFile)
        {
            project = new ProjectData();
            project.Load(projectFile);
        }
/// <inheritdoc/>

        public void Save()
        {
            if(project != null)
                project.Save();
        }
/// <inheritdoc/>

        public void Load(string path, string proj)
        {
            project = new ProjectData(path, proj);
            project.Load();
        }

        /// <inheritdoc/>

        IInterfacedNodesFinder interfacedNodesFinder { get; } = new InterfacedNodesFinder();

        public Dictionary<int, List<int>> FindInterfacedNodes(List<int> nodes, int elemDim)
        {
            if (elemDim > 3 | elemDim < 0)
                throw new Exception("Неправильная размерность элементов");

            if(elemDim == 0)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.GetAllElements());
            else if (elemDim == 1)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E1DCollection.GetObjects());
            else if (elemDim == 2)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E2DCollection.GetObjects());
            else
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E3DCollection.GetObjects());
        }

        public Dictionary<int, List<int>> FindInterfacedNodes(int elemDim)
        {
            if (elemDim > 3 | elemDim < 0)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 0)
                return interfacedNodesFinder.Find(ModelData.ObjectData.GetAllElements());
            else if (elemDim == 1)
                return interfacedNodesFinder.Find(ModelData.ObjectData.E1DCollection.GetObjects());
            else if (elemDim == 2)
                return interfacedNodesFinder.Find(ModelData.ObjectData.E2DCollection.GetObjects());
            else
                return interfacedNodesFinder.Find(ModelData.ObjectData.E3DCollection.GetObjects());
        }

        /// <inheritdoc/>

        IExtractor2DFrom3D extractor2DFrom3D { get; } = new Extract2DFrom3D();

        public IEnumerable<Element2D> Create2DForm3D(int StartNumber)
        {
            return extractor2DFrom3D.Create(StartNumber, ModelData.ObjectData.E3DCollection.GetObjects());
        }

/// <inheritdoc/>

        IBoundaryEdgesFinder boundaryEdgesFinder { get; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes)
        {
            return boundaryEdgesFinder.CreateBoundaryEdges(linesNodes);
        }
        /// <inheritdoc/>
        public List<string> FindBoundaryEdges()
        {
            return boundaryEdgesFinder.Find(ModelData.ObjectData.GetAllElements().
                Select(x => (ISurfaceElement)x));
            
        }

        ISelectionHelper selectionHelper { get; } = new SelectionHelper();

        public List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color)
        {
            return selectionHelper.SelectNodeInDirection(ModelData.ObjectData, angleInGradus, numbFirst, numbSecond, color);
        }

        public void SelectNodeInPlane(Plane plane, Color color)
        {
            selectionHelper.SelectNodeInPlane(ModelData.ObjectData, plane, color);
        }
/// <inheritdoc/>

        public IFreeNodesFinder FreeNodesFinder { get; } = new FreeNodesFinder();

/// <inheritdoc/>

        public ICrossSectionMaker CrossSectionMaker { get; } = new CrossSection();
/// <inheritdoc/>

        public ICoincidentObjectsFinder CoincidentObjectsFinder { get; } = new FindCoincidentObjects();
/// <inheritdoc/>

        public IObjectsMerger ObjectsMerger { get; } = new MergeObjects();
/// <inheritdoc/>

        public ISurfaceMeshCreator SurfaceMeshCreator { get; set; }

/// <inheritdoc/>

        public IObjectsRenumber ObjectsRenumber { get; } = new ObjectsRenumber();
/// <inheritdoc/>

        public INormalCalculator NormalCalculator { get; } = new NormalsCalculator();
/// <inheritdoc/>

        public IVolumeMeshCreator VolumeMeshCreator { get; set; }

/// <inheritdoc/>

        public IExtractor1DFrom2D Extractor1DFrom2D { get; } = new Extract1DFrom2D();
        /// <inheritdoc/>
        public ITaskData TaskData => project.TaskData;
/// <inheritdoc/>

        public IModelData ModelData => project.ModelData;
        /// <inheritdoc/>
        public IGeneralData GeneralData => project.GeneralData;
    }
}
