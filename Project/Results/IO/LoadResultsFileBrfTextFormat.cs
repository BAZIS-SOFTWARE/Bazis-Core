using Project.Interfaces;
using Project.Interfaces.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Project.Results.IO
{
    /// <summary>
    /// LoadResultsFileBrfTextFormat
    /// </summary>
    public class LoadResultsFileBrfTextFormat : IResultsLoader
    {
        public event Action<object, ILoadEventArgs> LoadEvent;

        public IEnumerable<IResult> Load(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return ReadResults(sr);
            }
        }

        public List<IResult> ReadResults(StreamReader sr)
        {
            var line = sr.ReadLine();
            var format = new Dictionary<string, Dictionary<TaskKind, string[]>>();
            var results = new List<IResult>();

            while (line != "#Результаты")
            {
                if (line == "Формат")
                {
                    LoadEvent(this, new LoadEventArgs("Чтение формата результатов..."));

                    ReadResultDescription(sr, format);
                }
                if (line == "Termo")
                {
                    LoadEvent(this, new LoadEventArgs("Загрузка тепловых результатов..."));


                    results.AddRange(ReadResultsKind(sr, TaskKind.термическая, format));
                }
                if (line == "TermoMech")
                {
                    LoadEvent(this, new LoadEventArgs("Загрузка механических результатов..."));

                    results.AddRange(ReadResultsKind(sr, TaskKind.механическая, format));
                }
                line = sr.ReadLine();
            }
            LoadEvent(this, new LoadEventArgs("Результаты загружены"));
            return results;
        }

        private void ReadResultDescription(StreamReader sr, Dictionary<string, Dictionary<TaskKind, string[]>> format)
        {
            var line = sr.ReadLine();
            while (line != "#Формат")
            {
                var strAr = line.Split(' ');

                if (!format.ContainsKey(strAr[0]))
                {
                    if (strAr[1] == TaskKind.термическая.ToString())
                    {
                        format.Add(strAr[0], new Dictionary<TaskKind, string[]>() { { TaskKind.термическая, strAr } });
                    }
                    else if (strAr[1] == TaskKind.механическая.ToString())
                    {
                        format.Add(strAr[0], new Dictionary<TaskKind, string[]>() { { TaskKind.механическая, strAr } });
                    }
                    else format.Add(strAr[0], new Dictionary<TaskKind, string[]>() { { TaskKind.механическая, strAr } });
                }
                else
                {
                    if (strAr[1] == TaskKind.термическая.ToString())
                    {
                        format[strAr[0]].Add(TaskKind.термическая, strAr);
                    }
                    else if (strAr[1] == TaskKind.механическая.ToString())
                    {
                        format[strAr[0]].Add(TaskKind.механическая, strAr);
                    }
                    else format[strAr[0]].Add(TaskKind.механическая, strAr);
                }
                line = sr.ReadLine();
            }
        }

        public List<IResult> ReadResultsKind(StreamReader sr, TaskKind taskKind, Dictionary<string, Dictionary<TaskKind, string[]>> format)
        {
            //var resultsDataSetList = new List<ResultsDataSet>();
            //var timeList = new List<float>();
            var results = new List<IResult>();
            if (format.Count == 0)
                ReadOldResultDescription(format);

            var line = sr.ReadLine();
            while (line != "#" + taskKind)
            {

                if (line.Split(' ')[0] == "Время")
                {
                    var time = float.Parse(line.Split(' ')[1], CultureInfo.InvariantCulture);
                    var new_resultsDataSet = new DataSet();
                    //timeList.Add(time);
                    LoadEvent(this, new LoadEventArgs("Время " + time.ToString()));
                    Thread.Sleep(100);
                    while (line.Split(' ')[0] != "#Время")
                    {
                        if (line == "Узлы" | line == "Элементы")
                            ReadObjsResults(sr, taskKind, new_resultsDataSet, format[line]);
                        line = sr.ReadLine();
                    }
                    results.Add(new Result(new_resultsDataSet, time, taskKind));
                    //resultsDataSetList.Add(new_resultsDataSet);
                }
                line = sr.ReadLine();
            }
            //return new Tuple<List<float>, List<ResultsDataSet>>(timeList,resultsDataSetList);
            return results;
        }

        private void ReadObjsResults(StreamReader sr, TaskKind taskKind, DataSet new_resultsDataSet, Dictionary<TaskKind, string[]> dataNames)
        {
            string line = sr.ReadLine();
            while (line != "#" + dataNames[taskKind][0])
            {
                DataRow new_row;
                if (dataNames[taskKind][0] == "Узлы")
                    new_row = new_resultsDataSet.Tables["nodes"].NewRow();
                else new_row = new_resultsDataSet.Tables["elements"].NewRow();

                var dataValues = line.Split(' ');
                var dataNamesCut = new string[dataNames[taskKind].Length - 2];
                Array.Copy(dataNames[taskKind], 2, dataNamesCut, 0, dataNamesCut.Length);

                if (dataValues.Length > dataNamesCut.Length)
                    throw new Exception("Ошибка чтения данных " + "\"" + dataNames[taskKind][0] + "\"!" + " Значений больше чем названий!");

                AdaptDataFormat(dataValues, new_row, dataNamesCut);

                if (dataNames[taskKind][0] == "Узлы")
                    new_resultsDataSet.Tables["nodes"].Rows.Add(new_row);
                else new_resultsDataSet.Tables["elements"].Rows.Add(new_row);

                line = sr.ReadLine();
            }
        }

        private void AdaptDataFormat(string[] dataValues, DataRow dataRow, string[] dataNames)
        {
            dataRow[0] = int.Parse(dataValues[0]);

            var resCount = dataValues.Length;
            var colCount = dataRow.Table.Columns.Count;

            for (int i = 1; i < resCount; i++)
            {
                if (dataRow.Table.Columns.Contains(dataNames[i]))
                    dataRow[dataNames[i]] = float.Parse(dataValues[i], CultureInfo.InvariantCulture);
            }
        }

        private void ReadOldResultDescription(Dictionary<string, Dictionary<TaskKind, string[]>> format)
        {
            var resNodeTermo = "Узлы термическая Индекс T Q p1 p2 p3 p4 p5";
            var resElemTermo = "Элементы термическая Индекс T Q p1 p2 p3 p4 p5";
            var resNodeTermoMech = "Узлы механическая Индекс Ex Ey Ez Exy Exz Eyz Emis Emean Sx Sy" +
                " Sz Sxy Sxz Syz Smis Smean St T Q Ep Et Eex Eey Eez Eexy Eexz Eeyz p1 p2 p3 p4 p5";
            var resElemTermoMech = "Элементы механическая Индекс Ex Ey Ez Exy Exz Eyz Emis Emean Sx Sy" +
                " Sz Sxy Sxz Syz Smis Smean St T Q Ep Et Eex Eey Eez Eexy Eexz Eeyz p1 p2 p3 p4 p5";

            var taskDataNode = new Dictionary<TaskKind, string[]>()
                {
                    { TaskKind.термическая,resNodeTermo.Split(' ') },
                    { TaskKind.механическая,resNodeTermoMech.Split(' ') }
                };
            format.Add("Узлы", taskDataNode);
            taskDataNode = new Dictionary<TaskKind, string[]>()
                {
                    { TaskKind.термическая,resElemTermo.Split(' ') },
                    { TaskKind.механическая,resElemTermoMech.Split(' ') }
                };
            format.Add("Элементы", taskDataNode);
        }
    }
}
