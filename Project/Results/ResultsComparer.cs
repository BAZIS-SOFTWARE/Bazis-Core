using System.Collections.Generic;
using Project.Interfaces;

namespace Results.ResultsData
{
    public class ResultsComparer : IEqualityComparer<IResult>
    {
        public bool Equals(IResult x, IResult y)
        {
            return x.Time.Equals(y.Time) && x.TaskKind.Equals(y.TaskKind);
        }

        public int GetHashCode(IResult obj)
        {
            return (int)obj.Time;
        }
    }
}
