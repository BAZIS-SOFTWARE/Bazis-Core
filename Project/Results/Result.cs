using Project.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Project.Results
{
    /// <summary>
    /// Result
    /// </summary>
    public class Result : IResult
    {
        /// <inheritdoc/>
        public TaskKind TaskKind { get; set; }
/// <inheritdoc/>

        public float Time { get; }
/// <inheritdoc/>

        public DataSet Data { get; }
/// <inheritdoc/>

        public List<string> GetDataSchema(string dataName)
        {
            var table = Data.Tables[dataName].Clone();

            var schema = new List<string>(); 
            foreach (DataColumn column in table.Columns)
                schema.Add(column.ColumnName);
            return schema;
        }

        public Result(DataSet data, float time, TaskKind taskKind)
        {
            TaskKind = taskKind;
            Data = data;
            Time = time;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return TaskKind + " " + Time.ToString();
        }
/// <inheritdoc/>

        public float GetElementValue(int elemNumber, string resDescr)
        {
            var resu = Data.Tables["elements"].Rows.Find(elemNumber);
            if (resu != null)
            {
                return (float)resu[resDescr];
            }
            else return 0.0f;
        }
/// <inheritdoc/>

        public float GetNodeValue(int nodeNumber, string resDescr)
        {
            var resu = Data.Tables["nodes"].Rows.Find(nodeNumber);
            if (resu != null)
            {
                return (float)resu[resDescr];
            }
            else return 0.0f;
        }
/// <inheritdoc/>

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
    }

}
