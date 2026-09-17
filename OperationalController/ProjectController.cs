using Geometry;
using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.MeshObjects;
using OperationalController.GmshController;
using OperationalController.MeshObjsUtility;
using OperationalController.SurfaceMeshCreator;
using Project.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace OperationalController
{
    /// <summary>
    /// ProjectController
    /// </summary>
    public partial class ProjectController : IProjectController
    {
/// <inheritdoc/>

        IGmshController GmshController = new GmshController.GmshController();

        /// <summary>
        /// Возвращает признак готовности геометрического контроллера.
        /// </summary>
        public bool IsGeometryInitialized => GmshController.IsInitialized;

        /// <summary>
        /// Инициализирует контроллер геометрии для текущего проекта.
        /// </summary>
        /// <param name="gmshDllPath">Путь к библиотеке Gmsh.</param>
        public void InitializeGeometry(string gmshDllPath)
        {
            if (string.IsNullOrWhiteSpace(gmshDllPath))
                throw new ArgumentException("Gmsh DLL path is not specified.", nameof(gmshDllPath));

            if (GmshController.IsInitialized)
                throw new InvalidOperationException("Geometry controller is already initialized.");

            GmshController.Load(gmshDllPath);


            // спросить Николая насколько это корректно?
            // глобальная опция поведения Gmsh при ошибках
            /*
            0 — не прерывать выполнение;
            1 — прервать построение сетки;
            2 — выбросить исключение вне интерактивного режима;
            3 — всегда выбросить исключение;
            4 — завершить процесс.
             */
            GmshController.Gmsh.Option.SetNumber("General.AbortOnError", 0);

            if (!GmshController.IsInitialized)
                throw new InvalidOperationException("Gmsh initialization failed.");
        }

        /// <summary>
        /// Завершает работу контроллера геометрии.
        /// </summary>
        public void UnloadGeometry()
        {
            if (!GmshController.IsInitialized)
                return;

            GmshController.Unload();
        }

        IProjectData project;

        ISelectionHelper selectionHelper { get; set; } = new SelectionHelper();
/// <inheritdoc/>

        public List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color)
        {
            return selectionHelper.SelectNodeInDirection(ModelData.ObjectData, angleInGradus, numbFirst, numbSecond, color);
        }
        /// <inheritdoc/>
        public List<int> SelectNodeInPlane(Geometry.Plane plane, Color color)
        {
            return selectionHelper.SelectNodeInPlane(ModelData.ObjectData, plane, color);
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

        public void AddTaskData(ICondData cond)
        {
            TaskData?.Add(cond);
        }



        /// <inheritdoc/>
        ITaskData TaskData => project?.TaskData;
/// <inheritdoc/>

        IModelData ModelData => project?.ModelData;

        IGeneralData GeneralData => project?.GeneralData;

        /// <inheritdoc/>
        public string Name 
        {
            get => GeneralData.Name;
            set => GeneralData.Name = value;
        }
    }
}
