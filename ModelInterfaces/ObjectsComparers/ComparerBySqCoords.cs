using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.ObjectsComparers
{
    public class ComparerBySqCoords : IComparer<IModelObject>
    {
        public int Compare(IModelObject x, IModelObject y)
        {
            var x_centrP = x.CalcCentralPoint();
            var y_centrP = y.CalcCentralPoint();

            var x_sqSum = x_centrP.GetSquareSumCoords();
            var y_sqSum = y_centrP.GetSquareSumCoords();

            return x_sqSum.CompareTo(y_sqSum);       
        }
    }
}
