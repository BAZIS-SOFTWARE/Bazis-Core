using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Interfaces.Tasks;
using Project.Interfaces.IO;
using Model.Interfaces;
using System;
using Project.Tasks.IO;
using Model.GroupsData;


namespace Project.Tasks
{

    public class TaskData :  ITaskData
    {
        List<IPhysicalData> taskData;

        public int Count { get { return taskData.Count; } }

        public bool IsReadOnly => throw new System.NotImplementedException();
/// <inheritdoc/>

        public ITaskDataSaver Saver { get; set; } = new SaveTaskDataTextFormat();
        /// <inheritdoc/>
        public ITaskDataLoader Loader { get; set; } = new LoadTaskDataTextFormat();

        public IPhysicalData this[int ind]
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
            taskData = new List<IPhysicalData>();
        }
/// <inheritdoc/>

        public IEnumerable<T> Find<T>() where T : IPhysicalData
        {
            foreach (var data in taskData)
            {
                if (data is T)
                    yield return (T)data;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IPhysicalData> Find(DataKind kind)
        {
            foreach (var data in taskData)
            {
                if (data.Kind == kind)
                    yield return data;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IPhysicalData> Find(string grName)
        {
            foreach (var data in taskData)
            {
                if (data.Group.Name == grName)
                    yield return data;
            }
        }
        /// <inheritdoc/>

        public void Add(IPhysicalData data)
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

        public int IndexOf(IPhysicalData item)
        {
            return taskData.IndexOf(item);
        }
/// <inheritdoc/>

        public void Insert(int index, IPhysicalData item)
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

        public bool Contains(IPhysicalData item)
        {
            return taskData.Find(x => x.Kind == item.Kind & x.ToString().Equals(item.ToString())) == null ? false : true;
        }
/// <inheritdoc/>

        public void CopyTo(IPhysicalData[] array, int arrayIndex)
        {
            taskData.CopyTo(array, arrayIndex);
        }
/// <inheritdoc/>

        public bool Remove(IPhysicalData item)
        {
            return taskData.Remove(item);
        }

        public int RemoveAll(DataKind kind)
        {
            return taskData.RemoveAll(x => x.Kind == kind);
        }

        public IEnumerator<IPhysicalData> GetEnumerator()
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

        public void AddRange(IEnumerable<IPhysicalData> datas)
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
        public void ClearNotExisted(IGroup group)
        {
            var existData = taskData.Where(x => x.Group == group).ToList();

            foreach (var item in existData)
                taskData.Remove(item);
        }
        /// <inheritdoc/>

        public void ClearNotExisted(IGroupData gropData)
        {
            //var valData = taskData.Select(x => (IPhysicalData)x);

            var existData = taskData.Where(x => gropData.Contains(x.Group)).ToList();

            taskData.Clear();
            taskData.AddRange(existData);
        }
/// <inheritdoc/>

        public IPhysicalData Create(string name, string info,IGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (name == "Материал" | name == "Материалы")
            {
                return new MatData(group, info);
            }

            else if (name == "Среда")
            {
                return new MediaData(group, info);
            }

            else if (name == "Нагрузка")
            {
                return new LoadData(group, info);
            }

            else if (name == "Нагрев")
            {
                //dataInfo = dataInfo.Replace("*", "0");
                return new HeatData(group, info);
            }

            else if (name == "Закрепления" | name == "Закрепление")
            {
                return new ClampData(group, info);
            }
            else return null;
        }
/// <inheritdoc/>

        public bool Contains(string grName)
        {
            return taskData.FirstOrDefault(x => x.Group.Name == grName)
                == null ? false : true;
        }
    }
}
