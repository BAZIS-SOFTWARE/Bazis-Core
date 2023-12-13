using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;

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
                CreatePresenter(objsItem.Value, objsItem.Key);
            }
        }
/// <inheritdoc/>

        public void CreatePresenter(List<IModelObject> modelObjects, ObjType objsType)
        {
            switch (objsType)
            {
                case ObjType.Узел:
                case ObjType.Точка:
                    var pointsPresentor = CreatePointObjectsPresenter(modelObjects);
                    presentors.Add(objsType.ToString(), pointsPresentor);
                    break;
                case ObjType.Линия:
                    var linesPresentor = CreateGeometryLinePresenter(modelObjects.Select(x => (ILineObject<IPoint>)x));
                    presentors.Add(objsType.ToString(), linesPresentor);
                    break;
                case ObjType.Фигура2D:
                    var fig2dPresentor = CreateSurfaceGeometryPresenter(modelObjects.Select(x => (ISurfaceObject<IGeometrySurface, IPoint>)x), false);
                    presentors.Add(objsType.ToString(), fig2dPresentor);
                    break;
                case ObjType.Элемент1D:
                    var els1dPresentor = CreateBeamPresenter(modelObjects.Select(x => (ILineObject<INode>)x));
                    presentors.Add(objsType.ToString(), els1dPresentor);
                    break;
                case ObjType.Элемент2D:
                    var els2dPresentor = CreateSurfaceElementsPresenter(modelObjects.Select(x => (ISurfaceObject<IElementSurface, INode>)x), false);
                    presentors.Add(objsType.ToString(), els2dPresentor);
                    break;
                case ObjType.Элемент3D:
                    var els3dPresentor = CreateSurfaceElementsPresenter(modelObjects.Select(x => (ISurfaceObject<IElementSurface,INode>)x), true);
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
            return new PointObjsPresenter(modelObjects);
        }

/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceElementsPresenter(IEnumerable<ISurfaceObject<IElementSurface, INode>> surfaceObjs, bool isVolumeObjs)
        {
            return new SurfaceObjsPresenter<IElementSurface, INode>(surfaceObjs, isVolumeObjs);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceGeometryPresenter(IEnumerable<ISurfaceObject<IGeometrySurface, IPoint>> surfaceObjs, bool isVolumeObjs)
        {
            return new SurfaceObjsPresenter<IGeometrySurface, IPoint>(surfaceObjs, isVolumeObjs);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateBeamPresenter(IEnumerable<ILineObject<INode>> lineObjs)
        {
            return new LineObjsPresenter<INode>(lineObjs);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateGeometryLinePresenter(IEnumerable<ILineObject<IPoint>> lineObjs)
        {
            return new LineObjsPresenter<IPoint>(lineObjs);
        }
    }
}
