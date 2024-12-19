using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Results;
using Project.Results.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Results.ResultsData
{
    public class ResultData : IResultData
    {
        List<IResult> results;

        public ResultData()
        {
            results = new List<IResult>();
        }

        public IResult this[int ind]
        {
            get { return results[ind]; }
            set { results[ind] = value; }
        }
/// <inheritdoc/>

        public void Load(string path)
        {
            var results = Loader.Load(path);

            this.results.Clear();
            this.results.AddRange(results);
        }              
/// <inheritdoc/>

        public List<TaskKind> GetResultKinds()
        {
            var resKinds = new List<TaskKind>();

            foreach (var result in results)
            {
                if (!resKinds.Contains(result.TaskKind))
                    resKinds.Add(result.TaskKind);
            }
            return resKinds;
        }
/// <inheritdoc/>

        public int Count { get { return results.Count; } }
/// <inheritdoc/>

        public bool IsReadOnly { get; } = false;
        /// <inheritdoc/>
        public IResultsSaver Saver { get; set; } = new SaveResultsFileDb();
        /// <inheritdoc/>
        public IResultsLoader Loader { get; set; } = new LoadResultsFileDB();

        public void Add(float time, DataSet data, TaskKind taskKind)
        {
            var res = results.Find(x => x.Time.Equals(time) & x.TaskKind == taskKind);
            if (res != null)
            {
                results.Add(new Result(data, time, taskKind));
            }
        }
/// <inheritdoc/>

        public void AddRange(IEnumerable<IResult> results)
        {
            this.results.AddRange(results);           
        }

        public void AddRange(IEnumerable<IResult> results, IEqualityComparer<IResult> comparer)
        {
            this.results.AddRange(results);
            var distResults = this.results.Distinct(comparer).ToList();
            this.results = distResults;
        }

        public IEnumerable<IResult> GetResults()
        {
            foreach (var result in results) 
                yield return result;
        }

        public IEnumerator<IResult> GetEnumerator()
        {
            foreach (var result in results) yield return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void RemoveRange(int startInd, int stopInd)
        {
            results.RemoveRange(startInd, stopInd);
        }
/// <inheritdoc/>


        public void Add(IResult results)
        {
            this.results.Add(results);
        }

        public void RemoveByTaskKind(TaskKind taskKind)
        {
            var res = results.Where(x => x.TaskKind == taskKind).ToList();
            for (int i = 0; i < res.Count(); i++)
            {
                results.Remove(res[i]);
            }
        }

        public void RemoveRange(string taskKind, float firstTime, float secondTime)
        {
            var findRes = FindByTimeInterval(taskKind, firstTime, secondTime);

            foreach (var res in findRes)
                results.Remove(res);
        }
/// <inheritdoc/>

        public IEnumerable<IResult> FindByTimeInterval(string taskKind, float firstTime, float lastTime)
        {
            var findRes = results.Where(x => x.TaskKind.ToString() == taskKind &&

            x.Time.CompareTo(firstTime) >= 0
            & x.Time.CompareTo(lastTime) <= 0);

            return findRes;
        }
        /// <summary>
        /// FindByTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        /// <returns></returns>
        public IEnumerable<IResult> FindByTaskKind(TaskKind taskKind)
        {
            var findRes = results.Where(x => x.TaskKind == taskKind);
            return findRes;
        }
/// <inheritdoc/>

        public IEnumerable<IResult> FindByTaskKind(string taskKind)
        {
            var findRes = results.Where(x => x.TaskKind.ToString() == taskKind);
            return findRes;
        }
/// <inheritdoc/>

        public IResult FindByTime(TaskKind taskKind, float time)
        {
            var findRes = results.Find(x => x.TaskKind == taskKind && x.Equals(time));

            return findRes;
        }
/// <inheritdoc/>

        public IResult FindByTime(TaskKind taskKind, float time, float delta)
        {
            var findRes = results.Find(x => x.TaskKind == taskKind && Math.Abs(x.Time - time) < delta);

            return findRes;
        }
        /// <inheritdoc/>

        public IResult FindByTime(string taskKind, float time)
        {
            var findRes = results.Find(x => x.TaskKind.ToString() == taskKind && x.Time.Equals(time));

            return findRes;
        }
/// <inheritdoc/>

        public IResult FindByTime(string taskKind, float time, float delta)
        {
            var findRes = results.Find(x => x.TaskKind.ToString() == taskKind && Math.Abs(x.Time - time) < delta);

            return findRes;
        }


/// <inheritdoc/>

        public int IndexOf(IResult item)
        {
            return results.IndexOf(item);
        }
/// <inheritdoc/>

        public void Insert(int index, IResult item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void RemoveAt(int index)
        {
            if (results.Count > index)
                results.RemoveAt(index);
        }
/// <inheritdoc/>

        public void Clear()
        {
            results.Clear();
        }
/// <inheritdoc/>

        public bool Contains(IResult item)
        {
            return results.Find(x => x.TaskKind == item.TaskKind & x.Time.Equals(item.Time)) == null ? false : true;
        }
/// <inheritdoc/>

        public void CopyTo(IResult[] array, int arrayIndex)
        {
            results.CopyTo(array, arrayIndex); 
        }
/// <inheritdoc/>

        public bool Remove(IResult item)
        {
            return results.Remove(item);
        }
    }
}
