using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsUtility;
using OperationalController.SurfaceMeshCreator;
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
using Model.IO;
using Project.Tasks;

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

        IBoundaryEdgesFinder boundaryEdgesFinder { get; set; } = new FindBoundaryEdges();
        /// <inheritdoc/>

        public IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes)
        {
            return boundaryEdgesFinder.CreateBoundaryEdges(linesNodes, ModelData);
        }
        /// <inheritdoc/>
        public List<string> FindBoundaryEdges()
        {
            if (ModelData.ObjectData.GetAllElements().Count() == 0)
                throw new Exception("Модель не содержит элементов");

            return boundaryEdgesFinder.Find(ModelData.ObjectData.GetAllElements().
                Select(x => (ISurfaceElement)x));       
        }

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

        IExtractor1DFrom2D extractor1DFrom2D { get; set; } = new Extract1DFrom2D();
/// <inheritdoc/>

        public bool Create1DFrom2D()
        {
            if (ModelData.ObjectData.E2DCollection.Count == 0)
                return false;
            //return extractor2DFrom3D.Create(StartNumber, ModelData.ObjectData.E3DCollection.GetObjects());
            var coll = ModelData.ObjectData.E1DCollection;

            var number = coll.Count != 0 ? coll.GetMaxNumber() + 1 : 0;

            var name = $"boundary1DSet_{number}";

            var e1d = extractor1DFrom2D.Create(number, ModelData.ObjectData.E2DCollection.GetObjects().ToArray());
            ModelData.ObjectData.E1DCollection.AddRange(name, e1d);

            return true;
        }
        
/// <inheritdoc/>

        public void SetModelObjectsBackColor(ObjType objType)
        {
            ModelData?.ObjectData?.SetBackColor(objType);
        }
        /// <inheritdoc/>
        public void SaveAs(string projectFile)
        {
            var ext = System.IO.Path.GetExtension(projectFile);

            if (ext == ".bpf2")
                ModelData.Saver = new SaveModelToBPF2TextFile();
            else if (ext == ".bpf")
                ModelData.Saver = new SaveModelToBPFTextFile();
            else
                throw new Exception("неизвестное расширение файла");

            Save();
        }
        /// <inheritdoc/>
        public IEnumerable<IGroup> GetAllModelGroups()
        {
            return ModelData.GroupData;
        }
        /// <inheritdoc/>
        public IEnumerable<ICondData> GetAllCondData()
        {
            return TaskData;
        }


        /// <inheritdoc/>
        public ITaskData TaskData => project?.TaskData;
/// <inheritdoc/>

        public IModelData ModelData => project?.ModelData;

        IGeneralData GeneralData => project?.GeneralData;
        /// <inheritdoc/>
        public string Path 
        { 
            get => GeneralData.Path;
            set => GeneralData.Path = value;
        }
        /// <inheritdoc/>
        public string Name 
        {
            get => GeneralData.Name;
            set => GeneralData.Name = value;
        }


    }
}
