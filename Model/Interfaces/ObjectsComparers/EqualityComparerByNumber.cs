using Model.Interfaces;
using System.Collections.Generic;

namespace Model.Interfaces.ObjectsComparers
{
    public class EqualityComparerByNumber<T> : IEqualityComparer<T> where T : IModelObject
    {

        public bool Equals(T x, T y)
        {
            if (x.Number == y.Number)
                return true;
            else return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.Number;
        }
    }
}
