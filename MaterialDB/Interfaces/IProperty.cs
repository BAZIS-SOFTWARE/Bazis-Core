using MaterialDB.MaterialData;
using System.Data;

namespace MaterialDB.Interfaces
{
    /// <summary>
    /// IProperty
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// this
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        double this[int row, int col] {  get; set; }
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
        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="copyName"></param>
        /// <returns></returns>
        PropertyItem Copy(string copyName);


        void AddRow();
    }
}
