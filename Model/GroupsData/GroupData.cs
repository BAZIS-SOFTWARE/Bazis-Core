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

        public int Count
        {
            get { return groups.Count; }
        }


        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add(IGroup item)
        {
            groups.Add(item);
        }

        public void Clear()
        {
            groups.Clear();
        }

        public bool Contains(IGroup item)
        {
            return groups.Contains(item, new NameAndTypeGroupComparer());
        }

        public void CopyTo(IGroup[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<IGroup> GetEnumerator()
        {
            foreach (var group in groups)
            {
                yield return group;
            }
        }

        public int IndexOf(IGroup item)
        {
            return groups.IndexOf(item);
        }

        public void Insert(int index, IGroup item)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IGroup item)
        {
            return groups.Remove(item);
        }

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
            return groups.Find(x => x.GroupName == grName);
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
                    var gr = new Group(item.GroupName, item.ObjType);
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
    }
}