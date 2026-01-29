using OperationalController.MeshObjsUtility;
using OperationalController.SurfaceMeshCreator;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Interfaces;
using Geometry;
using Model.GeometryObjects;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System;
using OperationalController.GmshController;

namespace OperationalController
{
    /// <summary>
    /// ProjectController
    /// </summary>
    public partial class ProjectController : IProjectController
    {
/// <inheritdoc/>

        public IGmshController GmshController { get; set; } = new GmshController.GmshController();

        IProjectData project;

        ISelectionHelper selectionHelper { get; set; } = new SelectionHelper();
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

        IFreeNodesFinder freeNodesFinder { get; set; } = new FreeNodesFinder();
        /// <inheritdoc/>
        public IEnumerable<int> FindFreeNodes()
        {
            return freeNodesFinder.Find(ModelData.ObjectData);
        }

/// <inheritdoc/>

        ICrossSectionMaker crossSectionMaker { get; set; } = new CrossSection();
/// <inheritdoc/>

        public SurfaceFigure GetSectionSurfaces(Geometry.Plane plane)
        {
            return crossSectionMaker.GetSectionSurfaces(ModelData.ObjectData.E3DCollection.GetObjects(), plane);
        }

        ICoincidentObjectsFinder coincidentObjectsFinder { get; set; } = new FindCoincidentObjects();
        /// <inheritdoc/>
        public List<List<int>> FindCoincidentObjects(ObjType objType, float distance)
        {
            var objs = ModelData.ObjectData.GetObjects(objType).ToList();
            coincidentObjectsFinder.ProgressEvent += (ar1, ar2) => 
            { 
                MessageEvent?.Invoke(ar2.ToString()); 
            };
            return coincidentObjectsFinder.Find(objs, distance);
        }

        IObjectsMerger objectsMerger { get; set; } = new MergeObjects();

        /// <inheritdoc/>
        public void MergeNodes(List<List<int>> numbers)
        {
            objectsMerger.Merge(numbers, ModelData.ObjectData.NodesSet);
        }

        ISurfaceMeshCreator surfaceMeshCreator { get; set; }

/// <inheritdoc/>

        IObjectsRenumber objectsRenumber { get; set; } = new ObjectsRenumber();
/// <inheritdoc/>

        INormalCalculator normalCalculator { get; set; } = new NormalsCalculator();
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

        public void SetModelObjectsBackColor(ObjType objType)
        {
            ModelData?.ObjectData?.SetBackColor(objType);
        }


        /// <inheritdoc/>
        public ITaskData TaskData => project?.TaskData;
/// <inheritdoc/>

        public IModelData ModelData => project?.ModelData;

        IGeneralData GeneralData => project?.GeneralData;

        /// <inheritdoc/>
        public string Name 
        {
            get => GeneralData.Name;
            set => GeneralData.Name = value;
        }
    }
}
