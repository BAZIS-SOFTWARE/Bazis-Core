using System.Collections.Generic;
using Project.Interfaces;
using Project.Results;

namespace Results.ResultsData
{
    public class ResultsComparer : IEqualityComparer<Result>
    {
        public bool Equals(Result x, Result y)
        {
            return x.Time.Equals(y.Time) && x.TaskKind.Equals(y.TaskKind);
        }

        public int GetHashCode(Result obj)
        {
            return (int)obj.Time;
        }
    }
}
