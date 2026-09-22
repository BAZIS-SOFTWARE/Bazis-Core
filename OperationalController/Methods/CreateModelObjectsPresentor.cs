using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.GeometryObjects;
using Model.MeshObjects;
using OperationalController.ModelScenePresentator;

namespace OperationalController
{
    public partial class ProjectController
    {
        private readonly PresentersCreator presentersCreator = new PresentersCreator();

        /// <summary>
        /// Создает презентатор видимых объектов заданного набора.
        /// </summary>
        public IObjsPresenter CreateModelObjectsPresentor(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            IObjsPresenter presenter;

            switch (setInfo.ObjType)
            {
                case ObjType.Узел:
                    presenter = CreatePointPresenter(ModelData.ObjectData.NodesSet.Values, setInfo);
                    break;
                case ObjType.Кривая:
                    presenter = CreateLinePresenter(ModelData.ObjectData.CurveSet.Values, setInfo);
                    break;
                case ObjType.Поверхность:
                    presenter = CreateGeometrySurfacePresenter(ModelData.ObjectData.SurfaceSet.Values, setInfo);
                    break;
                case ObjType.Элемент1D:
                    presenter = CreateLinePresenter(ModelData.ObjectData.E1DCollection[setInfo.Name].Values, setInfo);
                    break;
                case ObjType.Элемент2D:
                    presenter = CreateElementSurfacePresenter(ModelData.ObjectData.E2DCollection[setInfo.Name].Values, setInfo);
                    break;
                case ObjType.Элемент3D:
                    presenter = CreateElementSurfacePresenter(ModelData.ObjectData.E3DCollection[setInfo.Name].Values, setInfo);
                    break;
                default:
                    presenter = CreatePointPresenter(ModelData.ObjectData.PointsSet.Values, setInfo);
                    break;
            }

            presenter.ViewMode = ModelView.GetViewMode(setInfo);
            presenter.Name = setInfo.Name;
            return presenter;
        }

        private IObjsPresenter CreatePointPresenter(IEnumerable<IModelObject> objects, ISetInfo setInfo)
        {
            var pointObjects = CreateVisibleObjects(objects, setInfo);
            var colors = CreateColors(pointObjects, setInfo);
            return presentersCreator.CreatePointObjectsPresenter(pointObjects, colors);
        }

        private IObjsPresenter CreateLinePresenter<T>(IEnumerable<T> objects, ISetInfo setInfo) where T : IModelObject
        {
            var lineObjects = CreateVisibleObjects(objects, setInfo);
            var colors = CreateColors(lineObjects, setInfo);
            return presentersCreator.CreateLineObjectsPresenter(lineObjects, colors);
        }

        private IObjsPresenter CreateGeometrySurfacePresenter(IEnumerable<SurfaceFigure> objects, ISetInfo setInfo)
        {
            var surfaceObjects = CreateVisibleObjects(objects, setInfo);
            var colors = CreateColors(surfaceObjects, setInfo);
            var modelSurfaceObjects = surfaceObjects.Cast<ISurfaceObject<GeometrySurface, GeometryPoint>>().ToList();
            return presentersCreator.CreateSurfaceObjectsPresenter<GeometrySurface, GeometryPoint>(modelSurfaceObjects, colors);
        }

        private IObjsPresenter CreateElementSurfacePresenter(IEnumerable<Element> objects, ISetInfo setInfo)
        {
            var surfaceObjects = CreateVisibleObjects(objects, setInfo);
            var colors = CreateColors(surfaceObjects, setInfo);
            var modelSurfaceObjects = surfaceObjects.Cast<ISurfaceObject<ElementSurface, Node>>().ToList();
            return presentersCreator.CreateSurfaceObjectsPresenter<ElementSurface, Node>(modelSurfaceObjects, colors, !ModelView.HideInsideSurfaces);
        }

        private List<T> CreateVisibleObjects<T>(IEnumerable<T> objects, ISetInfo setInfo) where T : IModelObject
        {
            var visibleObjects = new List<T>();

            foreach (var modelObject in objects)
            {
                if (ModelView.GetVisible(setInfo.ObjType, modelObject.Number))
                    visibleObjects.Add(modelObject);
            }

            return visibleObjects;
        }

        private List<Color> CreateColors<T>(IReadOnlyList<T> objects, ISetInfo setInfo) where T : IModelObject
        {
            var colors = new List<Color>(objects.Count);

            foreach (var modelObject in objects)
                colors.Add(ModelView.GetColor(setInfo, modelObject.Number));

            return colors;
        }
    }
}
