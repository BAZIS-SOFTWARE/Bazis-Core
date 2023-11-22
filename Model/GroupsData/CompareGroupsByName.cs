using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class CompareGroupsByName : IEqualityComparer<IGroup>
    {
        public bool Equals(IGroup x, IGroup y)
        {
            return (x.GroupName == y.GroupName && x.ObjType == y.ObjType);
        }


        public int GetHashCode(IGroup obj)
        {
            throw new NotImplementedException();
        }
    }
}
