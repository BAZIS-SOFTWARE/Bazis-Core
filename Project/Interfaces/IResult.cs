using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Project.Interfaces
{
    /// <summary>
    /// TaskKind
    /// </summary>
    public enum TaskKind
    {
        термическая,
        механическая,
        химическая
    }
    /// <summary>
    /// IResult
    /// </summary>
    public interface IResult : ISerializable
    {
        TaskKind TaskKind { get; set; }

        float Time { get; }

        DataSet Data { get; }

        List<string> GetDataSchema(string dataName);

        float GetElementValue(int elemNumber, string resDescr);

        float GetNodeValue(int nodeNumber, string resDescr);
    }
}