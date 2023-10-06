using Functions.Sort;
using Model.Interfaces;
using Model.ObjectsSorters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class Group : IList<IModelObject>
    {
        List<IModelObject> objects = new List<IModelObject>();
        /// <summary>
        /// ViewState
        /// </summary>
        public bool ViewState { get; set; } = true;
        /// <summary>
        /// GroupName
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// ObjType
        /// </summary>
        public string ObjType { get; }

        
/// <inheritdoc/>

        public int Count
        {
            get { return objects.Count; }
        }

        public bool IsReadOnly => false;


        public IModelObject this[int key]
        {
            get { return objects[key]; }
            set { objects[key] = value; }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var indStr = string.Join(" ", objects.Select(x => x.Number));
            return string.Format("{0} {1} {2} | {3}", GroupName, ObjType, objects.Count, indStr);
        }


        public void Clear()
        {
            objects.Clear();
        }

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var item in objects)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(IModelObject item)
        {
            return objects.IndexOf(item);
        }

        public void Insert(int index, IModelObject item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            objects.RemoveAt(index);
        }

        public void Add(IModelObject item)
        {
            objects.Add(item);
        }

        public void AddRange(IEnumerable<IModelObject> items)
        {
            objects.AddRange(items);
        }

        public bool Contains(IModelObject item)
        {
            return objects.Contains(item);
        }

        public void CopyTo(IModelObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IModelObject item)
        {
            return objects.Remove(item);
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

        public bool Swap(int fInd, int sInd)
        {
            return objects.SwapList(fInd, sInd);
        }

        public void SortByDistance()
        {
            objects.SortByDistance();
        }
    }
}
