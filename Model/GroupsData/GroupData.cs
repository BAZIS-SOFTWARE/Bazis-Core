using ModelInterfaces;
using ModelInterfaces.ObjectsComparers;
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
            groups.Add(item);
        }
/// <inheritdoc/>

        public void Clear()
        {
            groups.Clear();
        }
/// <inheritdoc/>

        public bool Contains(IGroup item)
        {
            return groups.Contains(item, new NameAndTypeGroupComparer());
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
                    var gr = new Group(item.Name, item.ObjType);
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

        public IGroup Create(string name, ObjType objType)
        {
            return new Group(name, objType);
        }
/// <inheritdoc/>

        public IGroup Create(IEnumerable<IModelObject> modelObjects)
        {
            var objType = modelObjects.First().ObjType;

            if (!modelObjects.All(x => x.ObjType == objType))
                throw new System.Exception("Все объекты должны быть одного типа!");

            var grps = FindMany(objType);

            var counter = 1;
            var name = $"{objType}_{grps.Count() + counter}";

            while (true)
            {
                if (Find(name) != null)
                {
                    counter++;
                    name = $"{objType}_{grps.Count() + counter}";
                }
                else break;
            }

            var group = Create(name, objType);

            group.AddRange(modelObjects);
            return group;
        }
    }
}