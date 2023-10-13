using Geometry;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        public bool IsReadOnly => throw new NotImplementedException();

        public IObjsPresenter this[string key] 
        { 
            get { return presentors[key]; } 
            set { presentors[key] = value; }
        }


        /// <summary>
        /// ModelScenePresentator
        /// </summary>
        /// <param name="modelData"></param>
        public ModelScenePresentator(ModelData modelData)
        {
            var objsTypes = modelData.ObjectData.GetObjectTypes();

            //var sortedObjs = GetSortedObjects(modelData.ObjectData.ToArray());

            presentors = new Dictionary<string, IObjsPresenter>();

            foreach (var objsType in objsTypes)
            {
                switch (objsType)
                {
                    case "Узлы":
                        var pointObjsCreator = new PointObjsPresenter(modelData.ObjectData.FindMany(objsType).ToArray());
                        presentors.Add(objsType, pointObjsCreator);
                        break;
                    case "Элементы1D":
                    case "Линии":
                        var lineObjs = modelData.ObjectData.Where(x => x.ObjType == objsType).
    Select(x => (ILineObject)x);
                        var lineObjsCreator = new LineObjsPresenter(lineObjs.ToArray());
                        presentors.Add(objsType, lineObjsCreator);
                        break;
                    case "Элементы2D":
                    case "Элементы3D":
                    case "Поверхности":
                        var surfaceObjs = modelData.ObjectData.Where(x => x.ObjType == objsType).
    Select(x => (ISurfaceObject)x);
                        var surfaceObjsCreator = new SurfaceObjsPresenter(surfaceObjs.ToArray());
                        presentors.Add(objsType, surfaceObjsCreator);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <inheritdoc/>

        public Tuple<int, int, int,int> CreateVBOIndexes(string objsType)
        {
            if (ContainsKey(objsType))
            {
                var presenter = presentors[objsType];
                return presenter.CreateIndexes();
            }
            else throw new Exception("Объект не найден!");
        }
        /// <inheritdoc/>
        public float[] CreateVBOVertexes(string objsType, int vertexLength, string vertexType)
        {
            if (ContainsKey(objsType))
            {
                var presenter = presentors[objsType];
                return presenter.CreateVertexes(vertexLength, vertexType);
            }
            else throw new Exception("Объект не найден!");
        }
/// <inheritdoc/>

        public bool[] CreateVBOEdges(string objsType, int vertexLength)
        {
            if (ContainsKey(objsType))
            {
                var presenter = presentors[objsType];
                return presenter.CreateEdgeFlags(vertexLength);
            }
            else throw new Exception("Объект не найден!");
        }
/// <inheritdoc/>

        public int[] CreateVBOPointers(string objsType, int indexLength)
        {
            if (ContainsKey(objsType))
            {
                var presenter = presentors[objsType];
                return presenter.CreatePointers(indexLength);
            }
            else throw new Exception("Объект не найден!");
        }

/// <inheritdoc/>

        public void HideInsideObjects(string objsType)
        {
            if(ContainsKey(objsType))
            {
                var presenter = presentors[objsType];

                if (presenter is SurfaceObjsPresenter surfaceObjsCreator)
                    surfaceObjsCreator.HideInsideSurfaces();
            }
            else throw new Exception("Объект не найден!");
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

        public bool ContainsKey(string objsType)
        {
            return presentors.ContainsKey(objsType);
        }

        public void Add(string objsType, IObjsPresenter presenter)
        {
            if(!presentors.ContainsKey(objsType))
                presentors.Add(objsType, presenter);
        }

        public bool Remove(string objsType)
        {
            return presentors.Remove(objsType);
        }

        public bool TryGetValue(string objsType, out IObjsPresenter presenter)
        {
            return presentors.TryGetValue(objsType, out presenter);
        }

        public void Add(KeyValuePair<string, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            presentors.Clear();
        }

        public bool Contains(KeyValuePair<string, IObjsPresenter> item)
        {
            throw new NotImplementedException();
        }

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

        public IObjsPresenter CreatePresenter(ModelObject[] modelObjects)
        {
            var objsType = modelObjects.First().ObjType;

            if(objsType == "Узлы")
            {
                return new PointObjsPresenter(modelObjects);
            }
            else if(objsType == "Элементы1D" | objsType == "Линии")
            {
                var lineObjs = modelObjects.Select(x => (ILineObject)x);
                return new LineObjsPresenter(lineObjs.ToArray());
            }
            else
            {
                var surfaceObjs = modelObjects.Select(x => (ISurfaceObject)x);
                return new SurfaceObjsPresenter(surfaceObjs.ToArray());
            }            
        }
    }
}
