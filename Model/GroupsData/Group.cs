using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class Group : IList<int>
    {
        List<int> objNumbers = new List<int>();

        public bool ViewState { get; set; } = true;

        public int this[int ind]
        {
            get { return objNumbers[ind]; }
            set { objNumbers[ind] = value; }
        }

        public override string ToString()
        {
            var indStr = string.Join(" ", objNumbers.Select(x => x.ToString()));
            return string.Format("{0} {1} {2} | {3}", GroupName, ObjType, objNumbers.Count, indStr);
        }

        public Group(string groupName, ObjKind objKind)
        {
            GroupName = groupName;

            if (objKind == ObjKind.Node)
                ObjType = "Узлы";
            else if (objKind == ObjKind.Tetra)
                ObjType = "Элементы3D";
            else if (objKind == ObjKind.Triangle)
                ObjType = "Элементы2D";
            else
                ObjType = "Поверхности";

            ViewState = true;
        }

        public Group(string groupName, string objType)
        {
            GroupName = groupName;
            ObjType = objType;
            ViewState = true;
        }

        public IEnumerable<int> ObjsNumbers
        {
            get
            {
                foreach (var number in objNumbers)
                {
                    yield return number;
                }
            }
        }

        public void AddRange(IEnumerable<int> list)
        {

            objNumbers.AddRange(list);
        }

        public void Add(int ind)
        {
            objNumbers.Add(ind);
        }

        public int Count
        {
            get
            {
                return objNumbers.Count;
            }
        }

        public void ReverseIndex()
        {
            objNumbers.Reverse();
        }

        public string GroupName { get; set; }
        public string ObjType { get; internal set; }

        public bool IsReadOnly => throw new NotImplementedException();

        public bool Remove(int index)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            objNumbers.RemoveAt(index);
        }

        public void Clear()
        {
            objNumbers.Clear();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var number in objNumbers)
            {
                yield return number;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
