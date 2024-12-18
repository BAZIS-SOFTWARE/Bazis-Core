using Model.Interfaces;
using System.Collections.Generic;

namespace Model.Interfaces.ObjectsComparers
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
