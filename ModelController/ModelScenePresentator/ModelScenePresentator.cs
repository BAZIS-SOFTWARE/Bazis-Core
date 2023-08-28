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
    public class ModelScenePresentator : IEnumerable<KeyValuePair<string, ObjsPresenter>>
    {
        Dictionary<string, ObjsPresenter> presentors;


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
            var presenter = GetObjectsPresenter(objsType);
            return presenter.CreateIndexes();
        }
        /// <inheritdoc/>
        public float[] CreateVBOVertexes(string objsType, int vertexLength, string vertexType)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreateVertexes(vertexLength, vertexType);
        }
/// <inheritdoc/>

        public bool[] CreateVBOEdges(string objsType, int vertexLength)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreateEdgeFlags(vertexLength);
        }
/// <inheritdoc/>

        public int[] CreateVBOPointers(string objsType, int indexLength)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreatePointers(indexLength);
        }

/// <inheritdoc/>

        public void HideInsideObjects(string objsType)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            if (objsCreator is SurfaceObjsPresenter surfaceObjsCreator )
                surfaceObjsCreator.HideInsideSurfaces();
        }

    
/// <inheritdoc/>



        public IEnumerable<string> GetObjectsTypes()
        {
            foreach (var creator in presentors)
            {
                yield return creator.Key;
            }
        }

        public ObjsPresenter GetObjectsPresenter(string objsType)
        {
            if (!presentors.ContainsKey(objsType))
                return null;
            return presentors[objsType];
        }
        

/// <inheritdoc/>     
        public bool ContainsObjecs(string objsType)
        {
            return presentors.ContainsKey(objsType);
        }

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
    }
}
