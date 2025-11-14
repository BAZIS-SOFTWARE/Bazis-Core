using Model.Interfaces;
using System.Collections.Generic;

namespace Model.GroupsData
{
    public class NumberGroupComparer : IEqualityComparer<IGroup>
    {
        /// <inheritdoc/>
        public bool Equals(IGroup x, IGroup y)
        {
            return (x.Number == y.Number);
        }
        /// <inheritdoc/>


        public int GetHashCode(IGroup obj)
        {
            unchecked
            {
                return obj.Number;
            }
        }
    }
}
