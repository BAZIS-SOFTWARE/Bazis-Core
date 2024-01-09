using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelInterfaces.GeometryObjects;

namespace ModelController.ModelScenePresentator
{
    /// <summary>
    /// ModelScenePresentator
    /// </summary>
    public class ModelScenePresentator : IModelScenePresenter
    {
        Dictionary<string, IObjsPresenter> presentors;
/// <inheritdoc/>

        public ICollection<string> Keys { get{ return presentors.Keys; } } 
/// <inheritdoc/>

        public ICollection<IObjsPresenter> Values { get { return presentors.Values; } }
/// <inheritdoc/>

        public int Count { get { return presentors.Count; } }
/// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Indexator
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IObjsPresenter this[string key] 
        { 
            get { return presentors[key]; } 
            set { presentors[key] = value; }
        }

        public ModelScenePresentator()
        {
            presentors = new Dictionary<string, IObjsPresenter>();
        }
        /// <summary>
        /// ModelScenePresentator
        /// </summary>
        /// <param name="objectsData"></param>
        public ModelScenePresentator(IObjectsData objectsData) : this()
        {
            foreach (var objsItem in objectsData)
            {
                AddObjects(objsItem.Value, objsItem.Key);
            }
        }
/// <inheritdoc/>

        public void AddObjects(List<IModelObject> modelObjects, ObjType objsType)
        {
            switch (objsType)
            {
                case ObjType.Узел:
                case ObjType.Точка:
                    var pointsPresentor = new PointObjsPresenter();
                    pointsPresentor.Add(modelObjects);
                    presentors.Add(objsType.ToString(), pointsPresentor);
                    break;
                case ObjType.Линия:
                    var linePres = new LineObjsPresenter<IGeometryPoint>();
                    var lines = modelObjects.Select(x => (ILineObject<IGeometryPoint>)x);
                    linePres.Add(lines);
                    presentors.Add(objsType.ToString(), linePres);
                    break;
                case ObjType.Фигура2D:
                    var fig2dPresentor = new SurfaceObjsPresenter<IGeometrySurface, IPoint>(false);
                    var figs2d = modelObjects.Select(x => (ISurfaceObject<IGeometrySurface, IPoint>)x);
                    fig2dPresentor.Add(figs2d);
                    presentors.Add(objsType.ToString(), fig2dPresentor);
                    break;
                case ObjType.Элемент1D:
                    var els1dPresentor = new LineObjsPresenter<INode>();
                    var els1d = modelObjects.Select(x => (ILineObject<INode>)x);
                    els1dPresentor.Add(els1d);
                    presentors.Add(objsType.ToString(), els1dPresentor);
                    break;
                case ObjType.Элемент2D:
                    var els2dPresentor = new SurfaceObjsPresenter<IElementSurface, INode>(false);
                    var els2d = modelObjects.Select(x => (ISurfaceObject<IElementSurface, INode>)x);
                    els2dPresentor.Add(els2d);
                    presentors.Add(objsType.ToString(), els2dPresentor);
                    break;
                case ObjType.Элемент3D:
                    var els3dPresentor = new SurfaceObjsPresenter<IElementSurface, INode>(true);
                    var els3d = modelObjects.Select(x => (ISurfaceObject<IElementSurface, INode>)x);
                    els3dPresentor.Add(els3d);
                    presentors.Add(objsType.ToString(), els3dPresentor);
                    break;             
            }
        }    


        /// <inheritdoc/>

        public IEnumerator<KeyValuePair<string, IObjsPresenter>> GetEnumerator()
        {
            foreach (var presentor in presentors)
            {
                yield return new KeyValuePair<string, IObjsPresenter>(presentor.Key, presentor.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public bool ContainsKey(string presentorName)
        {
            return presentors.ContainsKey(presentorName);
        }
/// <inheritdoc/>

        public void Add(string presentorName, IObjsPresenter presenter)
        {
            if(!presentors.ContainsKey(presentorName))
                presentors.Add(presentorName, presenter);
        }
/// <inheritdoc/>

        public bool Remove(string presentorName)
        {
            return presentors.Remove(presentorName);
        }
/// <inheritdoc/>

        public bool TryGetValue(string presentorName, out IObjsPresenter presenter)
        {
            return presentors.TryGetValue(presentorName, out presenter);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<string, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void Clear()
        {
            presentors.Clear();
        }

        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="presentorName"></param>

        public void Clear(string presentorName)
        {
            presentors[presentorName].Clear();
        }
        /// <inheritdoc/>
        public void ClearNotExisted()
        {
            foreach (var presentor in presentors)
                presentor.Value.ClearNotExisted();
        }

    /// <inheritdoc/>

    public bool Contains(KeyValuePair<string, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void CopyTo(KeyValuePair<string, IObjsPresenter>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public bool Remove(KeyValuePair<string, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects)
        {
            var pointsPresentor = new PointObjsPresenter();
            pointsPresentor.Add(modelObjects);
            return pointsPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateBeamPresenter(IEnumerable<ILineObject<INode>> lineObjs)
        {
            var els1dPresentor = new LineObjsPresenter<INode>();
            els1dPresentor.Add(lineObjs);
            return els1dPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateGeometryLinePresenter(IEnumerable<ILineObject<IGeometryPoint>> lineObjs)
        {
            var linePres = new LineObjsPresenter<IGeometryPoint>();
            linePres.Add(lineObjs);
            return linePres;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceElementsPresenter(IEnumerable<ISurfaceObject<IElementSurface, INode>> surfaceElems, bool isVolumeObjs)
        {
            var elsPresentor = new SurfaceObjsPresenter<IElementSurface, INode>(isVolumeObjs);
            elsPresentor.Add(surfaceElems);
            return elsPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceGeometryPresenter(IEnumerable<ISurfaceObject<IGeometrySurface, IPoint>> surfaceGeom, bool isVolumeObjs)
        {
            var fig2dPresentor = new SurfaceObjsPresenter<IGeometrySurface, IPoint>(isVolumeObjs);
            fig2dPresentor.Add(surfaceGeom);
            return fig2dPresentor;
        }
    }
}
