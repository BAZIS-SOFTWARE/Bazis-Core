using MaterialDB.MaterialData;
using Project.Interfaces.Tasks;
using Project.Tasks.FrameCreators;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Community.CsharpSqlite.Sqlite3;

namespace Project.Tasks.Functions
{
    /// <summary>
    /// ParametersFunction
    /// </summary>
    public abstract class ParameterFunction : Function
    {

        /// <inheritdoc/>

        public override string ToString()
        {
            var str = string.Join(",", parameters);
            return $"PF({Name},{str})";
        }

    }
}
