using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ModelController.MeshObjsUtility
{
    public class FindCoincidentObjects
    {
        public event Action<object, float> ProgressEvent;
        ObjectsData objectData;
        float distance;
        List<int> tempNumbers = new List<int>();
        public FindCoincidentObjects(ObjectsData objectData, float distance)
        {
            this.objectData = objectData;
            this.distance = distance;
        }

        public List<List<int>> Find<T>() where T : ModelObject
        {
            var double_distance = distance * distance;

            var objs = objectData.FindMany<T>().ToArray();

            foreach (var obj in objs)
                obj.ViewState = false;

            var objGrps = new List<List<int>>();

            var progress = 0.0f;
            var progressStep = objs.Length / 10.0f;

            for (int i = 0; i < objs.Length; i++)
            {
                if (!objs[i].ViewState)
                {
                    var temp = new List<int>() { objs[i].Number };
                    var point_i = objs[i].CalcCentralPoint();

                    for (int j = i + 1; j < objs.Length; j++)
                    {
                        var point_j = objs[j].CalcCentralPoint();
                        var vector = point_j.Sub(point_i);

                        var square = vector._x * vector._x + vector._y * vector._y + vector._z * vector._z;

                        if (square < double_distance)
                        {
                            objs[i].MasterColor = Color.Red;
                            objs[i].ViewState = true;
                            objs[j].MasterColor = Color.Red;
                            objs[j].ViewState = true;
                            temp.Add(objs[j].Number);
                        }
                    }
                    objGrps.Add(temp);
                }

                if (i >= progress)
                {
                    ProgressEvent(this, progress / objs.Length);
                    progress += progressStep;
                }
            }
            return objGrps;
        }       
    }
}
