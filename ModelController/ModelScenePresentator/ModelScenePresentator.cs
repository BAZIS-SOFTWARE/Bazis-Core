using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Elements;

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
            var objsTypes = objectsData.GetObjectTypes();

            foreach (var objsType in objsTypes)
            {
                switch (objsType)
                {
                    case "Узлы":
                        var pointsPresentor = CreatePointObjectsPresenter(objectsData.FindMany(objsType));
                        presentors.Add(objsType, pointsPresentor);
                        break;
                    case "Элементы1D":
                        var els1dPresentor = CreateLineObjectsPresenter(objectsData.FindMany<Beam>());
                        presentors.Add(objsType, els1dPresentor);
                        break;
                    case "Линия":
                        var linesPresentor = CreateLineObjectsPresenter(objectsData.FindMany<Line>());
                        presentors.Add(objsType, linesPresentor);
                        break;
                    case "Элементы2D":
                        var els2dPresentor = CreateSurfaceObjectsPresenter(objectsData.FindMany<Element2D>());
                        presentors.Add(objsType, els2dPresentor);
                        break;
                    case "Элементы3D":
                        var els3dPresentor = CreateSurfaceObjectsPresenter(objectsData.FindMany<Element3D>());
                        presentors.Add(objsType, els3dPresentor);
                        break;
                    case "Поверхности":
                        var surfsPresentor = CreateSurfaceObjectsPresenter(objectsData.FindMany<Surface>());
                        presentors.Add(objsType, surfsPresentor);
                        break;
                }
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
