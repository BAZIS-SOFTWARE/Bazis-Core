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
        /// <summary>
        /// TaskKind
        /// </summary>
        TaskKind TaskKind { get; }
        /// <summary>
        /// Time
        /// </summary>
        float Time { get; }
        /// <summary>
        /// Data
        /// </summary>
        DataTable Data { get; }
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="objNumber"></param>
        /// <param name="resDescr"></param>
        /// <returns></returns>
        float GetValue(int objNumber, string resDescr);
        /// <summary>
        /// AddData
        /// </summary>
        /// <param name="dbDataTable"></param>
        void AddData(DataTable dbDataTable);

        //float GetNodeValue(int nodeNumber, string resDescr);
    }
}