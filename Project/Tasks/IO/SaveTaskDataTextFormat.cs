using System;
using System.IO;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;

namespace Project.Tasks.IO
{
    /// <summary>
    /// SaveTaskDataTextFormat
    /// </summary>
    public class SaveTaskDataTextFormat : ITaskDataSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaveEventArgs> SaveEvent;
/// <inheritdoc/>

        public void Save(ITaskData taskData, string path)
        {

            using (var sw = new StreamWriter(path, true, System.Text.Encoding.Unicode) { AutoFlush = true })
            {
                sw.WriteLine("");
                sw.WriteLine("Проект");
                foreach (var item in taskData)
                    sw.WriteLine(item.ToString());
                sw.WriteLine("#Проект");
            }
        }
    }
}
