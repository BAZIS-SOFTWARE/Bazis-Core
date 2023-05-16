using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Octree;
using System.Numerics;

namespace ModelController.MeshObjsUtility
{
    public class FindCoincidentObjects
    {
        public event Action<object, float> ProgressEvent;
        private ObjectsData objectData;
        private float distance;
        public FindCoincidentObjects(ObjectsData objectData, float distance)
        {
            this.objectData = objectData;
            this.distance = distance;
        }

        public List<List<int>> Find<T>() where T : ModelObject
        {
            var items = objectData.FindMany<T>().ToList();
            var tree = BuildTree(items);
            items.ForEach(obj => obj.ViewState = false);
            var result = new List<List<int>>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].ViewState)
                {
                    var point = items[i].CalcCentralPoint();
                    var neighbourElements = RadialSearch(point, distance, tree);
                    neighbourElements.ForEach(x => x.ViewState = true);
                    result.Add(neighbourElements.Select(x => x.Number).ToList());
                }
            }
            return result;
        }

        private PointOctree<T> BuildTree<T>(List<T> items) where T : ModelObject
        {
            var initialSize = 10;
            var tree = new PointOctree<T>(initialSize, Vector3.Zero, 1e-9f);
            foreach (var item in items)
            {
                var point = item.CalcCentralPoint();
                tree.Add(item, new Vector3(point._x, point._y, point._z));
            }
            return tree;
        }

        private List<T> RadialSearch<T>(Point3D point, float radius, PointOctree<T> tree) where T : ModelObject
        {
            return tree.GetNearby(new Vector3(point._x, point._y, point._z), radius)
                .ToList();
        }
    }
}
