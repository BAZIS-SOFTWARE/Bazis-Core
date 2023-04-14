using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class CompareGroupsByName : IEqualityComparer<Group>
    {
        public bool Equals(Group x, Group y)
        {
            return (x.GroupName == y.GroupName && x.ObjType == y.ObjType);
        }


        public int GetHashCode(Group obj)
        {
            throw new NotImplementedException();
        }
    }
}
