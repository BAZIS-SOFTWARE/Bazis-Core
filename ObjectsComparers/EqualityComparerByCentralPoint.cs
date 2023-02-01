using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ObjectsComparers
{
    public class EqualityComparerByCentralPoint : IEqualityComparer<ModelObject>
    {
        public bool Equals(ModelObject x, ModelObject y)
        {
            if (x.CalcCentralPoint() == y.CalcCentralPoint())
                return true;
            else return false;
        }

        public int GetHashCode(ModelObject obj)
        {
            return obj.Number;
        }
    }
}
