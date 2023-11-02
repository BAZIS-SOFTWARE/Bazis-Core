using ModelInterfaces;
using System.Collections.Generic;

namespace ModelInterfaces.ObjectsComparers
{
    public class EqualityComparerByNumber : IEqualityComparer<IModelObject>
    {

        public bool Equals(IModelObject x, IModelObject y)
        {
            if (x.Number == y.Number)
                return true;
            else return false;
        }

        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }
    }
}
