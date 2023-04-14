using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ObjectsComparers
{
    public class ComparerBySqCoords : IComparer<ModelObject>
    {
        public int Compare(ModelObject x, ModelObject y)
        {
            var x_centrP = x.CalcCentralPoint();
            var y_centrP = y.CalcCentralPoint();

            var x_sqSum = x_centrP.GetSquareSumCoords();
            var y_sqSum = y_centrP.GetSquareSumCoords();

            return x_sqSum.CompareTo(y_sqSum);       
        }
    }
}
