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
        Dictionary<ObjType, IObjsPresenter> presentors;
/// <inheritdoc/>

        public ICollection<ObjType> Keys { get{ return presentors.Keys; } } 
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
        public IObjsPresenter this[ObjType key] 
        { 
            get { return presentors[key]; } 
            set { presentors[key] = value; }
        }

        public ModelScenePresentator()
        {
            presentors = new Dictionary<ObjType, IObjsPresenter>();
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
                    presentors.Add(objsType, pointsPresentor);
                    break;
                case ObjType.Линия:
                case ObjType.Элемент1D:
                    var els1dPresentor = CreateLineObjectsPresenter(modelObjects.Select(x => (ILineObject)x));
                    presentors.Add(objsType, els1dPresentor);
                    break;
                case ObjType.Элемент2D:
                case ObjType.Элемент3D:
                case ObjType.Поверхность:
                    var els2dPresentor = CreateSurfaceObjectsPresenter(modelObjects.Select(x => (ISurfaceObject)x));
                    presentors.Add(objsType, els2dPresentor);
                    break;             
            }
        }

        /// <inheritdoc/>

        public void HideInsideSurfaces(IEnumerable<ISurfaceObject> surfaceElements)
        {
            var surfDict = new Dictionary<string, List<ISurface>>();

            foreach (var element in surfaceElements)
            {
                if (element.ViewState & element.NumberOfSides > 1)
                    foreach (var surface in element.GetSurface())
                    {
                        var numbers = surface.GetNodes().Select(x => x.Number).ToArray();

                        Array.Sort(numbers);
                        var surfInd = string.Join("", numbers);   //numbers[0].ToString() + numbers[1].ToString() + numbers[2].ToString();

                        if (!surfDict.ContainsKey(surfInd))
                        {
                            surface.ViewState = true;
                            surfDict.Add(surfInd, new List<ISurface>() { surface });
                        }

                        else
                        {
                            surface.ViewState = false;
                            surfDict[surfInd][0].ViewState = false;
                            surfDict[surfInd].Add(surface);
                        }
                    }
            }

        }
/// <inheritdoc/>

        public void ShowInsideSurfaces(IEnumerable<ISurfaceObject> surfaceElements)
        {
            foreach (var element in surfaceElements)
            {
                if (element.ViewState)
                    foreach (var surface in element.GetSurface())
                        surface.ViewState = true;
            }
        }


        /// <inheritdoc/>

        public IEnumerator<KeyValuePair<ObjType, IObjsPresenter>> GetEnumerator()
        {
            foreach (var presentor in presentors)
            {
                yield return new KeyValuePair<ObjType, IObjsPresenter>(presentor.Key, presentor.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public bool ContainsKey(ObjType presentorName)
        {
            return presentors.ContainsKey(presentorName);
        }
/// <inheritdoc/>

        public void Add(ObjType presentorName, IObjsPresenter presenter)
        {
            if(!presentors.ContainsKey(presentorName))
                presentors.Add(presentorName, presenter);
        }
/// <inheritdoc/>

        public bool Remove(ObjType presentorName)
        {
            return presentors.Remove(presentorName);
        }
/// <inheritdoc/>

        public bool TryGetValue(ObjType presentorName, out IObjsPresenter presenter)
        {
            return presentors.TryGetValue(presentorName, out presenter);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<ObjType, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void Clear()
        {
            presentors.Clear();
        }
/// <inheritdoc/>

        public bool Contains(KeyValuePair<ObjType, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void CopyTo(KeyValuePair<ObjType, IObjsPresenter>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public bool Remove(KeyValuePair<ObjType, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>


        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects)
        {
            return new PointObjsPresenter(modelObjects);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateLineObjectsPresenter(IEnumerable<ILineObject> lineObjs)
        {
            return new LineObjsPresenter(lineObjs);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceObjectsPresenter(IEnumerable<ISurfaceObject> surfaceObjs)
        {
            return new SurfaceObjsPresenter(surfaceObjs);
        }
    }
}
