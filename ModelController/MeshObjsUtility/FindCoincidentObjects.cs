using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Octree;
using System.Numerics;
using ModelInterfaces;

namespace ModelController.MeshObjsUtility
{
    public class FindCoincidentObjects
    {
        public event Action<object, float> ProgressEvent;
        private IObjectsData objectData;
        private float distance;

        public FindCoincidentObjects(IObjectsData objectData, float distance)
        {
            this.objectData = objectData;
            this.distance = distance;
        }

        /// <returns>
        /// Список групп элементов, находящихся на расстоянии друг от друга меньше заданного.
        /// </returns>
        public List<List<int>> Find<T>() where T : IModelObject
        {
            var items = objectData.FindMany<T>().ToList();
            var tree = BuildTree(items);
            items.ForEach(obj => obj.ViewState = false);
            var result = new List<List<int>>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].ViewState)
                {
                    var point = items[i].CalcCentr();
                    var neighbourElements = RadialSearch(point, distance, tree);
                    neighbourElements.ForEach(x => x.ViewState = true);
                    result.Add(neighbourElements.Select(x => x.Number).ToList());
                }
            }
            return result;
        }

        /// <summary>
        /// Построение октодерева из списка элементов
        /// </summary>
        private PointOctree<T> BuildTree<T>(List<T> items) 
            where T : IModelObject
        {
            var initialSize = GetModelSize(items);
            var initialPosition = GetCentralPoint(items);
            var tree = new PointOctree<T>(initialSize, initialPosition, 1e-6f);
            foreach (var item in items)
            {
                var point = item.CalcCentr();
                tree.Add(item, new Vector3(point._x, point._y, point._z));
            }
            return tree;
        }

        /// <summary>
        /// Возвращает все объекты октодерева, находящиеся внутри сферы заданного радиуса, 
        /// с центром в заданной точке
        /// </summary>
        private List<T> RadialSearch<T>(Point3D point, float radius, PointOctree<T> tree)
            where T : IModelObject
        {
            return tree.GetNearby(new Vector3(point._x, point._y, point._z), radius)
                .ToList();
        }

        /// <summary>
        /// Вычисляет центральную точку всей модели
        /// </summary>
        private Vector3 GetCentralPoint<T>(List<T> items)
            where T : IModelObject
        {
            var points = items.Select(i => i.CalcCentr());
            var x = points.Select(p => p._x).Sum() / points.Count();
            var y = points.Select(p => p._y).Sum() / points.Count();
            var z = points.Select(p => p._z).Sum() / points.Count();
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Вычисляет размер модели - диаметр сферы, которая описывает её
        /// </summary>
        private float GetModelSize<T>(List<T> items)
            where T : IModelObject
        {
            var points = items.Select(i => i.CalcCentr());
            var dx = Math.Abs(points.Select(p => p._x).Max() - points.Select(p => p._x).Min());
            var dy = Math.Abs(points.Select(p => p._y).Max() - points.Select(p => p._y).Min());
            var dz = Math.Abs(points.Select(p => p._z).Max() - points.Select(p => p._z).Min());
            return Math.Max(dx, Math.Max(dy, dz));
        }
    }
}