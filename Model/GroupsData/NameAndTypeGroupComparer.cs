using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class NameAndTypeGroupComparer : IEqualityComparer<IGroup>
    {
        /// <inheritdoc/>
        public bool Equals(IGroup x, IGroup y)
        {
            return (x.Name == y.Name && x.ObjType == y.ObjType);
        }
        /// <inheritdoc/>


        public int GetHashCode(IGroup obj)
        {
            unchecked
            {
                var hash = obj.Name.Length;
                hash *= hash;
                return 161 * hash;
            }
        }
    }
}
