using Geometry;
using ModelInterfaces;
using System.Collections.Generic;
using System.Drawing;

namespace ModelController
{
    public class ModelSceneSelector
    {
        IModelObject[] modelObjects;

        public string ObjsType {get;}

        public int NumberOfObjects { get { return modelObjects.Length; } }

        public ModelSceneSelector(IModelObject[] modelObjects, string objsType)
        {
            this.modelObjects = modelObjects;
            ObjsType = objsType;
        }

        public int GetObjNumber(int index)
        {
            return modelObjects[index].Number;
        }

        public IEnumerable<Point3D> GetObjPoints(int index)
        {
            return modelObjects[index].GetPoints();
        }

        public bool GetObjStatus(int index)
        {
            return modelObjects[index].ViewState;
        }

        public void SelectObject(int index, Color color)
        {
            modelObjects[index].MasterColor = color;
        }

        public void UnSelectObject(int index)
        {
            modelObjects[index].SetBackColor();
        }

        public IEnumerable<int> GetSelectedObjs(Color color)
        {          
            for (int i = 0; i < modelObjects.Length; i++)
            {
                if (modelObjects[i].MasterColor == color)
                    yield return modelObjects[i].Number;
            }
        }
    }
}
