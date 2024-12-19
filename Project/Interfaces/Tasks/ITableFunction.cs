using System.Data;

namespace Project.Interfaces.Tasks
{
    public interface ITableFunction
    {
        /// <summary>
        /// CalcProp y(x)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        float CalcProp(float param);

        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
            set;
        }
        /// <summary>
        /// X_unit
        /// </summary>

        string X_unit { get; set; }
        /// <summary>
        /// Y_unit
        /// </summary>

        string Y_unit { get; set; }

        /// <summary>
        /// Units
        /// </summary>
        string Units { get; set; }
        /// <summary>
        /// DataTable
        /// </summary>
        DataTable DataTable
        {
            get;
            set;
        }
    }
}
