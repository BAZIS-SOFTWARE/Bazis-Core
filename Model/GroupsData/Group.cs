using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class Group : IList<ModelObject>
    {
        List<ModelObject> objects = new List<ModelObject>();
        /// <summary>
        /// ViewState
        /// </summary>
        public bool ViewState { get; set; } = true;
        /// <summary>
        /// GroupName
        /// </summary>
        public string GroupName { get; }
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


        public ModelObject this[int key]
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

        public IEnumerator<ModelObject> GetEnumerator()
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

        public int IndexOf(ModelObject item)
        {
            return objects.IndexOf(item);
        }

        public void Insert(int index, ModelObject item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            objects.RemoveAt(index);
        }

        public void Add(ModelObject item)
        {
            objects.Add(item);
        }

        public bool Contains(ModelObject item)
        {
            return objects.Contains(item);
        }

        public void CopyTo(ModelObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ModelObject item)
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
    }
}
