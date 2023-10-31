using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ObjectsComparers
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
