using Project.Interfaces.Tasks;
using System;

namespace Project.Tasks
{
    /// <summary>
    /// Data
    /// </summary>
    public abstract class Data : IData
    {
        /// <inheritdoc/>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public DataKind Kind { get; internal set; }
/// <inheritdoc/>

        public abstract string GetInfo { get; }
        /// <summary>
        /// Data
        /// </summary>
        /// <param name="info"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public Data(string info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            else if (info.Length == 0)
                throw new Exception("Строка данных пустая!");

            SetInfo(info);
        }
        /// <inheritdoc/>

        public abstract void SetInfo(string info);
    }
}