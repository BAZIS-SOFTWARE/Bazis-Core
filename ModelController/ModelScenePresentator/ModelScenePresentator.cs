using Geometry;
using Model;
using Model.Interfaces;
using ModelController.MeshObjsUtility;
using ModelController.ModelScenePresentator.GlObjsPresenters;
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
    public class ModelScenePresentator : IDictionary<string, ObjsPresenter>
    {
        Dictionary<string, ObjsPresenter> presentors;
/// <inheritdoc/>

        public ICollection<string> Keys { get{ return presentors.Keys; } } 
/// <inheritdoc/>

        public ICollection<ObjsPresenter> Values { get { return presentors.Values; } }
/// <inheritdoc/>

        public int Count { get { return presentors.Count; } }
/// <inheritdoc/>

        public bool IsReadOnly => throw new NotImplementedException();

        public ObjsPresenter this[string key] 
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

            presentors = new Dictionary<string, ObjsPresenter>();

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
        public bool ContainsObjecs(string objsType)
        {
            return presentors.ContainsKey(objsType);
        }
/// <inheritdoc/>

        public IEnumerator<KeyValuePair<string, ObjsPresenter>> GetEnumerator()
        {
            foreach (var presentor in presentors)
            {
                yield return new KeyValuePair<string, ObjsPresenter>(presentor.Key, presentor.Value);
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

        public void Add(string objsType, ObjsPresenter presenter)
        {
            if(!presentors.ContainsKey(objsType))
                presentors.Add(objsType, presenter);
        }

        public bool Remove(string objsType)
        {
            return presentors.Remove(objsType);
        }

        public bool TryGetValue(string objsType, out ObjsPresenter presenter)
        {
            return presentors.TryGetValue(objsType, out presenter);
        }

        public void Add(KeyValuePair<string, ObjsPresenter> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            presentors.Clear();
        }

        public bool Contains(KeyValuePair<string, ObjsPresenter> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, ObjsPresenter>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, ObjsPresenter> item)
        {
            throw new NotImplementedException();
        }
    }
}
