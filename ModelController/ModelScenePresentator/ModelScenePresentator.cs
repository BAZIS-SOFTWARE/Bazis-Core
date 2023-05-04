using Geometry;
using Model;
using Model.Interfaces;
using ModelController.MeshObjsUtility;
using ModelController.ModelScenePresentator.GlObjsArraysCreators;
using SceneInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ModelController.ModelScenePresentator
{
    public class ModelScenePresentator : IEnumerable<KeyValuePair<string, ObjsPresenter>>, IScenePresentor
    {
        Dictionary<string, ObjsPresenter> presentors;

        ModelData modelData;

        public ModelScenePresentator(ModelData modelData)
        {
            this.modelData = modelData;
            var sortedObjs = GetSortedObjects(modelData.ObjectData.ToArray());

            presentors = new Dictionary<string, ObjsPresenter>();

            foreach (var sortedObj in sortedObjs)
            {
                if(sortedObj.Key == "Узлы")
                {
                    var pointObjsCreator = new PointObjsPresenter(sortedObjs[sortedObj.Key].ToArray());
                    presentors.Add("Узлы", pointObjsCreator);
                }
                else if(sortedObj.Key == "Элементы3D" | sortedObj.Key == "Элементы2D" | sortedObj.Key == "Поверхность")
                {
                    var surfaceObjs = sortedObjs[sortedObj.Key].Select(x => (ISurfaceObject)x);
                    var surfaceObjsCreator = new SurfaceObjsPresenter(surfaceObjs.ToArray());

                    presentors.Add(sortedObj.Key, surfaceObjsCreator);
                }
                else if(sortedObj.Key == "Элементы1D" | sortedObj.Key == "Линия")
                {
                    var lineObjs = sortedObjs[sortedObj.Key].Select(x => (ILineObject)x);
                    var lineObjsCreator = new LineObjsPresenter(lineObjs.ToArray());
                    presentors.Add(sortedObj.Key, lineObjsCreator);
                }
            }

        }   

        public Tuple<int, int, int> CreateVBOIndexes(string objsType)
        {
            var presenter = GetObjectsPresenter(objsType);
            return presenter.CreateIndexes();
        }

        public float[] CreateVBOVertexes(string objsType, int vertexLength, string vertexType)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreateVertexes(vertexLength, vertexType);
        }

        public bool[] CreateVBOEdges(string objsType, int vertexLength)
        {
            var objsCreator = (SurfaceObjsPresenter)GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreateEdgeFlags(vertexLength);
        }

        public int[] CreateVBOPointers(string objsType, int indexLength)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            return objsCreator.CreatePointers(indexLength);
        }

        public void HideInsideObjects(string objsType)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            if (objsCreator is SurfaceObjsPresenter surfaceObjsCreator)
                surfaceObjsCreator.HideInsideSurfaces();
            else if (objsCreator is PointObjsPresenter pointObjsPresenter)
            {
                // set normal to zero
                foreach (var node in modelData.ObjectData.FindMany<Node>())
                    node.Normal = new Point3D();

                var els3D = modelData.ObjectData.FindMany<Element3D>().Where(x => x.ViewState == true);
                var surfacePoints = pointObjsPresenter.FindSurfacePoints(els3D.ToArray());
                var surfacePointsNoRepeat = surfacePoints.Distinct().ToList();

                foreach (var point in pointObjsPresenter.GetObjs())
                    point.ViewState = false;

                foreach (var point in surfacePointsNoRepeat)
                    modelData.ObjectData.Find(point).ViewState = true;
            }
        }

        public void ShowInsideObjects(string objsType)
        {
            var objsCreator = GetObjectsPresenter(objsType);

            if (objsCreator == null)
                throw new Exception("Объект не найден!");

            if (objsCreator is SurfaceObjsPresenter surfaceObjsCreator)
            {
                surfaceObjsCreator.ShowInsideSurfaces();
            }

            else if (objsCreator is PointObjsPresenter pointObjsPresenter)
            {
                // set normal to zero
                foreach (var node in modelData.ObjectData.FindMany<Node>())
                    node.Normal = new Point3D();

                var els3D = modelData.ObjectData.FindMany<Element3D>().Where(x => x.ViewState == true);

                foreach (var el3D in els3D)
                    foreach (var node in el3D.GetNodes())
                        node.ViewState = true;
            }
        }

        public Dictionary<string, List<ModelObject>> GetSortedObjects(ModelObject[] modelObjects)
        {
            var sortedObjs = new Dictionary<string, List<ModelObject>>();
            foreach (var modelObject in modelObjects)
            {
                if (!sortedObjs.ContainsKey(modelObject.ObjType))
                    sortedObjs.Add(modelObject.ObjType, new List<ModelObject>() { modelObject });
                else
                    sortedObjs[modelObject.ObjType].Add(modelObject);
            }
            return sortedObjs;
        }

        public IEnumerable<string> GetObjectsTypes()
        {
            foreach (var creator in presentors)
            {
                yield return creator.Key;
            }
        }

        public int Count()
        {
            return presentors.Count();
        }

        private ObjsPresenter GetObjectsPresenter(string objsType)
        {
            if (!presentors.ContainsKey(objsType))
                return null;
            return presentors[objsType];
        }
        public void ShowAllObjects()
        {
            foreach (var presentor in presentors)
                foreach (var obj in presentor.Value.GetObjs())
                    obj.ViewState = true;
        }


        public void ShowObjects(string objsType)
        {
            foreach (var obj in presentors[objsType].GetObjs())
                obj.ViewState = true;
        }

        public void HideObjects(string objsType)
        {
            foreach (var obj in presentors[objsType].GetObjs())
            {
                obj.ViewState = false;
            }
        }

        public IEnumerable<Point3D> GetObjPoints(int index, string objsType)
        {
            return presentors[objsType][index].GetPoints();
        }

        public bool GetObjStatus(int index, string objsType)
        {
            return presentors[objsType][index].ViewState;
        }

        public void HideObjects(List<int> objsNumbs, string objsType)
        {    
            foreach (var objNumb in objsNumbs)
            {
                modelData.ObjectData.Find(objNumb).ViewState = false;
            }
        }

        public void HideObjects(Color color, string objsType)
        {
            foreach (var obj in presentors[objsType].GetObjs())
            {
                if (obj.MasterColor == color)
                    obj.ViewState = false;
            }
        }

        public void ColorObject(int index, Color color, string objsType)
        {
            presentors[objsType][index].MasterColor = color;
        }

        

        public bool ContainsObjecs(string objsType)
        {
            return presentors.ContainsKey(objsType);
        }

        public int CountObjecs(string objsType)
        {
            if (ContainsObjecs(objsType))
                return presentors[objsType].Count();
            else return 0;
        }

        public IEnumerable<int> FindObjectsNumbers(Color color, string objsType)
        {
            foreach (var obj in presentors[objsType].GetObjs())
            {
                if (obj.MasterColor == color)
                    yield return obj.Number;
            }
        }

        public void SetBackColorObject(int index, string objsType)
        {
            presentors[objsType][index].SetBackColor();
        }

        public void SetBackColorAllObjects()
        {
            foreach (var presentor in presentors)
            {
                foreach (var obj in presentor.Value.GetObjs())
                {
                    obj.SetBackColor();
                }
            }
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

        public GLObjType GetObjectsGlType(string objsType)
        {
            if (objsType == "Узлы")
            {
                return GLObjType.point;
            }
            else if (objsType == "Элементы3D" | objsType == "Элементы2D" | objsType == "Поверхность")
            {
                return GLObjType.triangle;
            }
            else return GLObjType.line;

        }

        public List<Line> FindBoundaries()
        {
            var creator = new FindBoundaryEdges(modelData);
            var boundary = creator.Find();

            var lines = new List<Line>();
            foreach (var line in boundary)
            {
                var n0 = int.Parse(line.Split(' ')[0]);
                var n1 = int.Parse(line.Split(' ')[1]);

                var node0 = (Node)modelData.ObjectData.Find(n0);
                var node1 = (Node)modelData.ObjectData.Find(n1);

                lines.Add(new Line(node0.Position, node1.Position));
            }
            return lines;
        }

        public Point3D GetObjCentre(int index, string objsType)
        {
            return presentors[objsType][index].CalcCentralPoint();
        }

        //public List<Point3D> GetObjectsCoords(string objsType)
        //{
        //    var objs = creators[objsType];

        //    var coords = new List<Point3D>();

        //    foreach (var obj in objs.GetObjs())
        //    {
        //        coords.AddRange(obj.GetPoints());
        //    }
        //    return coords;
        //}    
    }
}
