using Model.Interfaces;
using System.Collections.Generic;

namespace Model.ObjectsComparers
{
    public class EqualityComparerByExistState : IEqualityComparer<ModelObject>
    {
        public bool Equals(ModelObject x, ModelObject y)
        {
            if (x.ExistState == y.ExistState)
                return true;
            else return false;
        }

        public int GetHashCode(ModelObject obj)
        {
            return obj.Number;
        }
    }
}
