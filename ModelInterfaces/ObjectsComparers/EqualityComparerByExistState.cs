using ModelInterfaces;
using System.Collections.Generic;

namespace ModelInterfaces.ObjectsComparers
{
    public class EqualityComparerByExistState : IEqualityComparer<IModelObject>
    {
        public bool Equals(IModelObject x, IModelObject y)
        {
            if (x.ExistState == y.ExistState)
                return true;
            else return false;
        }

        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }
    }
}
