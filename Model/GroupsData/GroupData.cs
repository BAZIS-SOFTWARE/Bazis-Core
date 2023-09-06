using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model.GroupsData
{
    public class GroupData : IList<Group>
    {
        List<Group> groups;

        public GroupData()
        {
            groups = new List<Group>();
        }

        public Group this[int index]
        {
            get { return groups[index]; }
            set { groups[index] = value; }
        }

        public int Count
        {
            get { return groups.Count; }
        }


        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add(Group item)
        {
            groups.Add(item);
        }

        public void Clear()
        {
            groups.Clear();
        }

        public bool Contains(Group item)
        {
            return groups.Contains(item, new CompareGroupsByName());
        }

        public void CopyTo(Group[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<Group> GetEnumerator()
        {
            foreach (var group in groups)
            {
                yield return group;
            }
        }

        public int IndexOf(Group item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, Group item)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(Group item)
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

        public Group Find(int index)
        {
            return groups[index];
        }

        public Group Find(string grName)
        {
            return groups.Find(x => x.GroupName == grName);
        }

        public IEnumerable<Group> FindMany(string objType)
        {
            return groups.Where(x => x.ObjType == objType);
        }
    }
}