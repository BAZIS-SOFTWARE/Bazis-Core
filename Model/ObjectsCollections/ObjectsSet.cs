using Geometry;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.ObjectsCollections
{
    public class ObjectsSet<T>: IObjectsSet<T> where T : IModelObject
    {
/// <inheritdoc/>

        public string Name { get; internal set; }

        /// <summary>
        /// ObjectsSet
        /// </summary>
        /// <param name="name"></param>
        public ObjectsSet(string name)
        {
            Name = name;

            if (typeof(T) == typeof(Node))
                ObjType = ObjType.Узел;
            else if (typeof(T) == typeof(Beam))
                ObjType = ObjType.Элемент1D;
            else if (typeof(T) == typeof(Element2D))
                ObjType = ObjType.Элемент2D;
            else if (typeof(T) == typeof(Element3D))
                ObjType = ObjType.Элемент3D;
            else if (typeof(T) == typeof(GeometryPoint))
                ObjType = ObjType.Точка;
            else if (typeof(T) == typeof(Curve))
                ObjType = ObjType.Кривая;
            else
                ObjType = ObjType.Поверхность;

        }

        internal Dictionary<int,T> objectsSet = new Dictionary<int, T>();

        public T this[int key] { get => objectsSet[key]; set => objectsSet[key] = value; }
/// <inheritdoc/>

        public ICollection<int> Keys => objectsSet.Keys;
/// <inheritdoc/>

        public ICollection<T> Values => objectsSet.Values;
/// <inheritdoc/>

        public int Count => objectsSet.Count;
/// <inheritdoc/>

        public bool IsReadOnly => false;
/// <inheritdoc/>

        public ObjType ObjType { get; }
        /// <inheritdoc/>
        public Color Color { get; private set; } = Color.DarkGray;
        /// <inheritdoc/>
        public ViewMode ViewMode { get; private set; } = ViewMode.Point;
/// <inheritdoc/>

        public int NumberOfObjects => objectsSet.Count;
/// <inheritdoc/>

        public bool ViewState
        {
            get 
            { 
                return objectsSet.Values.Any(x => x.ViewState == true) 
                    ? true : false; 
            }
        }
/// <inheritdoc/>

        public int Number { get; set; }

        /// <inheritdoc/>

        public override string ToString()
        {
            return $@"{Name} {Color} {ViewMode} {objectsSet.Count}";
        }

/// <inheritdoc/>

        public void Append(IObjectsSet<T> appSet)
        {
            var shifting = 0;
            if (objectsSet.Count != 0)
            {
                var max = objectsSet.Max(x => x.Key);
                var min = appSet.Min(x => x.Key);

                if (min <= max)
                    shifting = max + 1;
            }

            foreach (var value in appSet)
            {
                var number = value.Value.Number + shifting;
                value.Value.SetNumber(number);
                objectsSet.Add(number, value.Value);
            }
        }

        /// <inheritdoc/>

        public void Add(int key, T value)
        {
            objectsSet.Add(key, value);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<int, T> item)
        {
            objectsSet.Add(item.Key, item.Value);
        }
/// <inheritdoc/>

        public void Clear()
        {
            foreach (var item in objectsSet)
                item.Value.ExistState = false;

            objectsSet.Clear();
        }
/// <inheritdoc/>

        public bool Contains(KeyValuePair<int, T> item)
        {
            return objectsSet.Contains(item);
        }
/// <inheritdoc/>

        public bool ContainsKey(int key)
        {
            return objectsSet.ContainsKey(key);
        }
/// <inheritdoc/>

        public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            foreach (var item in objectsSet)
            {
                yield return item;
            }
        }

/// <inheritdoc/>

        public bool Remove(int key)
        {
            return objectsSet.Remove(key);
        }
        /// <inheritdoc/>

        public bool Remove(KeyValuePair<int, T> item)
        {
            return objectsSet.Remove(item.Key);
        }
/// <inheritdoc/>

        public bool TryGetValue(int key, out T value)
        {
            return objectsSet.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public void SetViewMode(ViewMode viewMode)
        {
            if (ObjType != ObjType.Узел | ObjType != ObjType.Точка)
                ViewMode = viewMode;
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            var temp = new Dictionary<int, T>();
            //var notExistedNumbs = objectsSet.Where(x => x.Value.ExistState == false).Select(x => x.Key);
            var existed = objectsSet.Where(x => x.Value.ExistState == true);

            foreach (var item in existed)
            {
                temp.Add(item.Key, item.Value);
            }

            objectsSet = temp;
        }
        /// <inheritdoc/>
        public IEnumerable<string> GetObjectsInfo()
        {
            foreach (var item in objectsSet)
            {
                yield return item.Value.ToString();
            }
        }
        /// <inheritdoc/>
        public void SetColor(Color color)
        {
            Color = color;
            foreach (var item in objectsSet)
                item.Value.Color = color;
        }
        /// <inheritdoc/>
        public void SetColor(Color color, int number)
        {
            objectsSet[number].Color = color;
        }
        /// <inheritdoc/>
        public IEnumerable<Point3D> GetCoords(int number)
        {
            return objectsSet[number].GetCoordinates();
        }
/// <inheritdoc/>

        public IEnumerable<int> GetNumbers()
        {
            return objectsSet.Keys;
        }
        /// <inheritdoc/>
        public void SetViewState(int number,bool viewState)
        {
            objectsSet[number].ViewState = viewState;
        }
        /// <inheritdoc/>
        public void SetViewState(bool viewState)
        {
            foreach (var item in objectsSet)
                item.Value.ViewState = viewState;
        }
        /// <inheritdoc/>

        public bool GetViewState(int number)
        {
            return objectsSet[number].ViewState;
        }
/// <inheritdoc/>

        public virtual void SetBackColor(int number)
        {
            objectsSet[number].Color = Color;
        }

        /// <inheritdoc/>

        public virtual void SetBackColor()
        {
            foreach (var item in objectsSet)
                item.Value.Color = Color;
        }
        /// <inheritdoc/>

        public Point3D GetCentr(int number)
        {
            return objectsSet[number].CalcCentr();
        }
/// <inheritdoc/>

        public string GetObjectInfo(int number)
        {
            return objectsSet[number].ToString();
        }
    }
}
