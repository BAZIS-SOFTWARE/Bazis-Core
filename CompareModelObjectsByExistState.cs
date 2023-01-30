using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CompareModelObjectsByExistState : IEqualityComparer<ModelObject>
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
