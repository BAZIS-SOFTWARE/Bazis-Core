using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.GroupsData
{
    public class GroupData : IGroupData
    {
        List<IGroup> groups;

        public GroupData()
        {
            groups = new List<IGroup>();
        }

        public IGroup this[int index]
        {
            get { return groups[index]; }
            set { groups[index] = value; }
        }
/// <inheritdoc/>

        public int Count
        {
            get { return groups.Count; }
        }
        /// <inheritdoc/>


        public bool IsReadOnly => false;
/// <inheritdoc/>

        public void Add(IGroup item)
        {
            if (!Contains(item))
                groups.Add(item);
            else throw new System.Exception($"Группа с номером {item.Number} уже существует");
        }
/// <inheritdoc/>

        public void Clear()
        {
            groups.Clear();
        }
/// <inheritdoc/>

        public bool Contains(IGroup item)
        {
            return groups.Contains(item, new NumberGroupComparer());
        }
/// <inheritdoc/>

        public void CopyTo(IGroup[] array, int arrayIndex)
        {
            groups.CopyTo(array, arrayIndex);
        }
/// <inheritdoc/>

        public IEnumerator<IGroup> GetEnumerator()
        {
            foreach (var group in groups)
            {
                yield return group;
            }
        }
/// <inheritdoc/>

        public int IndexOf(IGroup item)
        {
            return groups.IndexOf(item);
        }
/// <inheritdoc/>

        public void Insert(int index, IGroup item)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public bool Remove(IGroup item)
        {
            return groups.Remove(item);
        }
/// <inheritdoc/>

        public void RemoveAt(int index)
        {
            groups.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public IGroup Find(int index)
        {
            return groups[index];
        }
/// <inheritdoc/>

        public IGroup Find(string grName)
        {
            return groups.Find(x => x.Name == grName);
        }
/// <inheritdoc/>

        public IEnumerable<IGroup> FindMany(ObjType objType)
        {
            return groups.Where(x => x.ObjType == objType);
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            var temp = new List<IGroup>();
            foreach (var item in groups)
            {
                var existObjects = item.Where(x => x.ExistState == true);

                if (existObjects.Count() > 0)
                {
                    var gr = new Group(item.Number,item.Name, item.ObjType);
                    gr.AddRange(existObjects);
                    temp.Add(gr);
                }
  
            }
            groups = temp;
        }
/// <inheritdoc/>

        public void Sort()
        {
            groups = groups.OrderBy(x => x, new StringNumberGroupComparer()).ToList();
        }
        /// <inheritdoc/>
        [Obsolete("Не использовать, так как он дублирует метод 'Add'")]
        public void Create(string name, ObjType objType)
        {
            var group = new Group(name, objType);
            groups.Add(group);
        }
/// <inheritdoc/>

        public void Create(IEnumerable<IModelObject> modelObjects)
        {
            var objType = modelObjects.First().ObjType;

            if (!modelObjects.All(x => x.ObjType == objType))
                throw new System.Exception("Все объекты должны быть одного типа!");

            var number = groups.Count == 0 ? 0 : GetMaxNumber() + 1;
            var name = $"newGroup_{objType}_{number}";        

            var group = new Group(number,name, objType);

            group.AddRange(modelObjects);
            groups.Add(group);
        }
/// <inheritdoc/>

        public void Create(string name, IEnumerable<IModelObject> modelObjects)
        {
            var objType = modelObjects.First().ObjType;

            if (!modelObjects.All(x => x.ObjType == objType))
                throw new System.Exception("Все объекты должны быть одного типа!");

            var number = groups.Count == 0 ? 0 : GetMaxNumber() + 1;
            //var name = $"newGroup_{objType}_{number}";

            var group = new Group(number,name, objType);

            group.AddRange(modelObjects);
            groups.Add(group);
        }
        /// <inheritdoc/>

        public int GetMaxNumber()
        {
            return groups.Max(x => x.Number);
        }
    }
}