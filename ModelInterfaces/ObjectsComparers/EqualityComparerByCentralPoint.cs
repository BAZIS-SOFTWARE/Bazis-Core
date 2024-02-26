using System.Collections.Generic;

namespace ModelInterfaces.ObjectsComparers
{
    public class EqualityComparerByCentralPoint : IEqualityComparer<IModelObject>
    {
        /// <inheritdoc/>
        public bool Equals(IModelObject x, IModelObject y)
        {
            if (x.CalcCentr() == y.CalcCentr())
                return true;
            else return false;
        }
/// <inheritdoc/>

        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }
    }
}
