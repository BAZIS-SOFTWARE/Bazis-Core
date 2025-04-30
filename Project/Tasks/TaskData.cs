using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Interfaces.Tasks;
using Project.Interfaces.IO;
using Model.Interfaces;
using System;
using Project.Tasks.IO;


namespace Project.Tasks
{

    public class TaskData :  ITaskData
    {
        List<IData> taskData;

        public int Count { get { return taskData.Count; } }

        public bool IsReadOnly => throw new System.NotImplementedException();
/// <inheritdoc/>

        public ITaskDataSaver Saver { get; set; } = new SaveTaskDataTextFormat();
        /// <inheritdoc/>
        public ITaskDataLoader Loader { get; set; } = new LoadTaskDataTextFormat();

        public IData this[int ind]
        {
            get
            {
                return taskData[ind];
            }
            set
            {
                taskData[ind] = value;
            }
        }
        /// <summary>
        /// Task data constructor
        /// </summary>
        public TaskData()
        {
            taskData = new List<IData>();
        }
/// <inheritdoc/>

        public IEnumerable<T> Find<T>() where T : IData
        {
            foreach (var data in taskData)
            {
                if (data is T)
                    yield return (T)data;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IData> Find(DataKind kind)
        {
            foreach (var data in taskData)
            {
                if (data.Kind == kind)
                    yield return data;
            }
        }
/// <inheritdoc/>

        public void Add(IData data)
        {
            taskData.Add(data);
        }
/// <inheritdoc/>

        public void RemoveRange(IEnumerable<string> dataInfo)
        {
            foreach (var data in dataInfo)
            {
                var ind = taskData.FindIndex(x => x.ToString() == data);
                taskData.RemoveAt(ind);
            }
        }
/// <inheritdoc/>

        public int IndexOf(IData item)
        {
            return taskData.IndexOf(item);
        }
/// <inheritdoc/>

        public void Insert(int index, IData item)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public void RemoveAt(int index)
        {
            taskData.RemoveAt(index);
        }
/// <inheritdoc/>

        public void Clear()
        {
            taskData.Clear();
        }
/// <inheritdoc/>

        public bool Contains(IData item)
        {
            return taskData.Find(x => x.Kind == item.Kind & x.ToString().Equals(item.ToString())) == null ? false : true;
        }
/// <inheritdoc/>

        public void CopyTo(IData[] array, int arrayIndex)
        {
            taskData.CopyTo(array, arrayIndex);
        }
/// <inheritdoc/>

        public bool Remove(IData item)
        {
            return taskData.Remove(item);
        }

        public int RemoveAll(DataKind kind)
        {
            return taskData.RemoveAll(x => x.Kind == kind);
        }

        public IEnumerator<IData> GetEnumerator()
        {
            foreach (var data in taskData)
            {
                yield return data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
/// <inheritdoc/>

        public void Load(ITaskData taskData)
        {
            this.taskData.Clear();
            this.taskData.AddRange(taskData);
        }
/// <inheritdoc/>

        public void AddRange(IEnumerable<IData> datas)
        {
            taskData.AddRange(datas);
        }
/// <inheritdoc/>

        public void Save(string projectFile)
        {
            Saver.Save(this,projectFile);
        }
/// <inheritdoc/>

        public void Load(string projectFile, IGroupData gropData)
        {
            var taskData = Loader.Load(projectFile, gropData);

            this.taskData.Clear();
            this.taskData.AddRange(taskData);
        }
/// <inheritdoc/>

        public void ClearNotExisted(IGroupData gropData)
        {
            var valData = taskData.Select(x => (IValuableData)x);

            var existData = valData.Where(x => gropData.Contains(x.Group)).ToList();

            taskData.Clear();
            taskData.AddRange(existData);
        }
/// <inheritdoc/>

        public IData Create(string name, string dataInfo, IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (name == "Материал" | name == "Материалы")
            {
                return new MatData(group, dataInfo);
            }

            else if (name == "Среда")
            {
                return new MediaData(group, dataInfo);
            }

            else if (name == "Нагрузка")
            {
                return new LoadData(group, dataInfo);
            }

            else if (name == "Нагрев")
            {
                dataInfo = dataInfo.Replace("*", "0");
                return new HeatData(group, dataInfo);
            }

            else if (name == "Закрепления" | name == "Закрепление")
            {
                return new ClampData(group,dataInfo);
            }
            else return null;
        }
    }
}
