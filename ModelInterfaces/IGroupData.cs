using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModelInterfaces
{
    public interface IGroupData: IList<IGroup>
    {
        IGroup Find(int index);

        IGroup Find(string grName);

        IEnumerable<IGroup> FindMany(string objType);
    }
}