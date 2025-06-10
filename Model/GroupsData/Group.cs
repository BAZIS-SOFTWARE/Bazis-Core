using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Interfaces.Attributes;
using Model.Interfaces.ObjectsComparers;
using Geometry;
using Model.Extensions;

namespace Model.GroupsData
{
    public class Group : IGroup
    {
        List<IModelObject> objects = new List<IModelObject>();
        /// <summary>
        /// ViewState
        /// </summary>
        public bool ViewState { get; set; } = true;
        /// <summary>
        /// GroupName
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ObjType
        /// </summary>
        public ObjType ObjType { get; }

        
/// <inheritdoc/>

        public int Count
        {
            get { return objects.Count; }
        }
/// <inheritdoc/>

        public bool IsReadOnly => false;

        /// <summary>
        /// this
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IModelObject this[int key]
        {
            get { return objects[key]; }
            set { objects[key] = value; }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var indStr = string.Join(" ", objects.Select(x => x.Number));
            return string.Format("{0} {1} {2} | {3}", Name, ObjType, objects.Count, indStr);
        }
/// <inheritdoc/>


        public void Clear()
        {
            objects.Clear();
        }
/// <inheritdoc/>

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var item in objects)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
/// <inheritdoc/>

        public int IndexOf(IModelObject item)
        {
            return objects.IndexOf(item);
        }
/// <inheritdoc/>

        public void Insert(int index, IModelObject item)
        {
            objects.Insert(index, item);
        }
/// <inheritdoc/>

        public void RemoveAt(int index)
        {
            objects.RemoveAt(index);
        }
/// <inheritdoc/>

        public void Add(IModelObject item)
        {
            objects.Add(item);
        }
/// <inheritdoc/>

        public void AddRange(IEnumerable<IModelObject> items)
        {
            objects.AddRange(items);
        }
/// <inheritdoc/>

        public bool Contains(IModelObject item)
        {
            return objects.Contains(item);
        }
/// <inheritdoc/>

        public void CopyTo(IModelObject[] array, int arrayIndex)
        {
            objects.CopyTo(array, arrayIndex);
        }
/// <inheritdoc/>

        public bool Remove(IModelObject item)
        {
            return objects.Remove(item);
        }

        public Group(string groupName, ObjType objType)
        {
            Name = groupName;
            ObjType = objType;
        }
/// <inheritdoc/>

        public bool Swap(int fInd, int sInd)
        {
            return objects.SwapList(fInd, sInd);
        }
        /// <inheritdoc/>
        [Warning("Предлагаю ипользовать метод SortByDistance() из Geometry")]
        public void SortByDistance()
        {
            objects.SortByDistance();
        }
/// <inheritdoc/>

        public void ClearRemoved()
        {
            var removedObjs = objects.Where(x => x.ExistState == false);

            var existObjects = objects.Except(removedObjs, new EqualityComparerByExistState()).ToList();
            objects = existObjects;
        }
    }
}
