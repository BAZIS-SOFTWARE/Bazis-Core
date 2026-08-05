using Newtonsoft.Json;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PreProc.Interfaces;
using Project.TaskParameters;

namespace PreProc
{
    /// <summary>
    /// TaskStatus
    /// </summary>
    public enum TaskStatus : int { выполнить, пропустить };
    /// <summary>
    /// PreProc. Препроцессор
    /// </summary>
    public class PreProc : IPreProc
    {
        private JsonSerializerSettings settingsSerializer = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
        };

        private Dictionary<DataKind, List<Tuple<float, float>>> TimeSeparators = new Dictionary<DataKind, List<Tuple<float, float>>>();

        private float[] AllTimeSeparators;
        private ProcessProperty processProp;
        private string _path;

        /// <inheritdoc/>
        public void CalcCompDataV1(IEnumerable<ICondData> data, ProcessType processType, string path)
        {
            SetUpTimeSeparators(data);
            processProp = new ProcessProperty { CommonTaskType = processType, TaskKind = TaskKind.термическая | TaskKind.механическая };
            _path = path;

            CreateFilesForComplexProcess(
                GetParameters<HeatData, MediaData, TermalParameters>(data),
                GetParameters<LoadData, ClampData, MechanicalParameters>(data));
        }

        /// <inheritdoc/>
        public void CalcCompDataV2(IEnumerable<ICondData> data, ProcessProperty property, string path)
        {
            SetUpTimeSeparators(data);
            processProp = property;
            _path = path;

            switch (processProp.TaskKind)
            {
                //case ComplexTaskType.химическая:
                //    CreateFilesForSingleProcess(processProp.CommonTaskType.ToString(), GetParameters<ChemicalData, MediaData, ChemicalParameters>(data));
                //    break;

                case TaskKind.термическая:
                    CreateFilesForSingleProcess(processProp.TaskKind.ToString(), GetParameters<HeatData, MediaData, TermalParameters>(data));
                    break;

                case TaskKind.механическая:
                    CreateFilesForSingleProcess(processProp.TaskKind.ToString(), GetParameters<LoadData, ClampData, MechanicalParameters>(data));
                    break;

                //case ComplexTaskType.химическая_термическая:
                //    CreateFilesForComplexProcess(
                //        GetParameters<ChemicalData, MediaData, ChemicalParameters>(data),
                //        GetParameters<HeatData, MediaData, TermalParameters>(data));
                //    break;

                case TaskKind.термическая | TaskKind.механическая:
                    CreateFilesForComplexProcess(
                        GetParameters<HeatData, MediaData, TermalParameters>(data),
                        GetParameters<LoadData, ClampData, MechanicalParameters>(data));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void SetUpTimeSeparators(IEnumerable<ICondData> valuableData)
        {
            var separators = new List<float>();

            foreach (var data in valuableData)
            {
                var startStop = Tuple.Create(data.StartTime, data.StopTime);
                if (!TimeSeparators.ContainsKey(data.Kind))
                    TimeSeparators.Add(data.Kind, new List<Tuple<float, float>>() { startStop });
                else
                    TimeSeparators[data.Kind].Add(startStop);
                separators.Add(startStop.Item1);
                separators.Add(startStop.Item2);
            }
            AllTimeSeparators = separators.Distinct().OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Выделяет необходимые LoadData и BoundData из data.
        /// Проверяет, начался ли процесс без BoundData
        /// Проверяет, входит ли время выполнения LoadData в BoundData
        /// </summary>
        /// <typeparam name="LoadData">параметр данных, встроенных в процесс BoundData</typeparam>
        /// <typeparam name="BoundData">параметр окружения, без которого нельзя начинать или заканчивать LoadData в процессе</typeparam>
        /// <typeparam name="ResultParam">Параметризованные данные, в которых указано время начала и конца конкретного LoadData или BoundData</typeparam>
        /// <param name="data">Список ValuableData, хранящий данные разных процессов</param>
        /// <returns>список с параметризованными данными</returns>
        public List<ResultParam> GetParameters<LoadData, BoundData, ResultParam>(IEnumerable<ICondData> data)
            where ResultParam : GeneralParameters, new()
            where LoadData : CondData
            where BoundData : CondData
        {
            var matData = data.Where(md => md.Kind == DataKind.Материал).
                OrderBy(md => md.StartTime).ToList();
            var loadData = data.Where(ld => ld is LoadData).
                OrderBy(ld => ld.StartTime).ToList();
            var boundData = data.Where(bd => bd is BoundData).
                OrderBy(bd => bd.StartTime).ToList();

            CheckLoadDataInsideBoundDataInterval<LoadData, MatData>(boundData, matData);
            CheckLoadDataInsideBoundDataInterval<LoadData, BoundData>(loadData, boundData);

            var pars = ParametrizeResultParameters<ResultParam>();

            var iniTemps = matData.
                Select((md, index) => new { Key = md.Group.Name,Value = 20.0})
    .ToDictionary(x => x.Key, x => x.Value);

            pars[0].InitTemp = iniTemps;

            return pars;
        }

        /// <summary>
        /// Параметризирует временные свойства созданного по dataItem ResultParam
        /// Задает значения ResultParam по шаблону
        /// </summary>
        /// <typeparam name="ResultParam">Параметр, созданный с помощью dataItem</typeparam>
        /// <returns>Список параметров по шаблону с указанным временем действия</returns>
        private List<ResultParam> ParametrizeResultParameters<ResultParam>()
            where ResultParam : GeneralParameters, new()
        {
            var result = new List<ResultParam>();
            for (var i = 0; i < AllTimeSeparators.Length - 1; i++)
            {
                var startStop = Tuple.Create(AllTimeSeparators[i], AllTimeSeparators[i + 1]);
                var parameter = new ResultParam();
                SetDraftParameters(parameter);
                parameter.TimeSettings.StartTime = startStop.Item1;
                parameter.TimeSettings.StopTime = startStop.Item2;

                if (TimeSeparators.ContainsKey(DataKind.Нагрев))
                    if (!TimeSeparators[DataKind.Нагрев].Any(x => x.Item1 == startStop.Item1 && x.Item2 == startStop.Item2))
                    {
                        parameter.TimeSettings.InitTimeStep = 0.5f;
                        parameter.TimeSettings.MaxTimeStep = 100f;
                    }

                result.Add(parameter);
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetDraftParameters<ResultParam>(ResultParam parameter)
            where ResultParam : GeneralParameters, new()
        {
            if (parameter is TermalParameters termalParameter)
            {
                switch (processProp.CommonTaskType)
                {
                    case ProcessType.Welding: WeldingTermDraft(termalParameter); break;
                    case ProcessType.Heating: HeatingTermDraft(termalParameter); break;
                    case ProcessType.Quenching: QuenchingTermDraft(termalParameter); break;
                    default: TemperingTermDraft(termalParameter); break;
                }
            }

            else if (parameter is MechanicalParameters mechParameter)
            {
                switch (processProp.CommonTaskType)
                {
                    case ProcessType.Welding: WeldingMechDraft(mechParameter); break;
                    case ProcessType.Heating: HeatingMechDraft(mechParameter); break;
                    case ProcessType.Quenching: HeatingMechDraft(mechParameter); break;
                    default: HeatingMechDraft(mechParameter); break;
                }
            }
        }

        private void HeatingMechDraft(MechanicalParameters mech)
        {
            mech.Iterations = 25;
            mech.SaveRate = 5;
            mech.MechanicalConvergence.Is_Physically_NonLinear = true;
            mech.MechanicalConvergence.MaterialPlasticityCoeff = 0.5f;
            mech.MechanicalConvergence.Is_Switched_Um = true;
            mech.MechanicalConvergence.Um = 0.5f;
            mech.MechanicalConvergence.DUm = 0.0005f;
            mech.MechanicalConvergence.SiStm = 1.5f;
            mech.MechanicalConvergence.PlasticityCriterion = 1.25f;
            mech.SolverSettings.Solver = "SOR_iterative";
            mech.SolverSettings.MaxIter = 100;
            mech.SolverSettings.Precision = 0.0001f;
            mech.SolverSettings.Relaxation = 1.25f;
            mech.SolverSettings.Priority = "Наивысший";
            mech.TimeSettings.InitTimeStep = 0.01f;
            mech.TimeSettings.MaxTimeStep = 100f;
            mech.TimeSettings.MinTimeStep = 0.0005f;
        }

        private void TemperingTermDraft(TermalParameters term)
        {
            term.MetallurgicalProcesses = new List<string>() { "Выдержка" };
            term.Iterations = 2;
            term.SaveRate = 1;
            term.TermalConvergence.Is_Switched_Tm = true;
            term.TermalConvergence.Tm = 1500;
            term.SolverSettings.Solver = "SOR_iterative";
            term.SolverSettings.MaxIter = 100;
            term.SolverSettings.Precision = 0.01f;
            term.SolverSettings.Relaxation = 1.05f;
            term.SolverSettings.Priority = "Наивысший";
            term.TimeSettings.InitTimeStep = 0.01f; // при охлаждении -- 0.5f
            term.TimeSettings.MaxTimeStep = 100f; // при охлаждении -- 100f
            term.TimeSettings.MinTimeStep = 0.0005f;
        }

        private void QuenchingTermDraft(TermalParameters term)
        {
            term.MetallurgicalProcesses = new List<string>() { "Охлаждение" };
            term.Iterations = 2;
            term.SaveRate = 1;
            term.TermalConvergence.Is_Switched_Tm = true;
            term.TermalConvergence.Tm = 1500;
            term.SolverSettings.Solver = "SOR_iterative";
            term.SolverSettings.MaxIter = 100;
            term.SolverSettings.Precision = 0.01f;
            term.SolverSettings.Relaxation = 1.05f;
            term.SolverSettings.Priority = "Наивысший";
            term.TimeSettings.InitTimeStep = 0.01f; // при охлаждении -- 0.5f
            term.TimeSettings.MaxTimeStep = 100f; // при охлаждении -- 100f
            term.TimeSettings.MinTimeStep = 0.0005f;
        }

        private void HeatingTermDraft(TermalParameters term)
        {
            term.MetallurgicalProcesses = new List<string>() { "Нагрев" };
            term.Iterations = 2;
            term.SaveRate = 1;
            term.TermalConvergence.Is_Switched_Tm = true;
            term.TermalConvergence.Tm = 1500;
            term.SolverSettings.Solver = "SOR_iterative";
            term.SolverSettings.MaxIter = 100;
            term.SolverSettings.Precision = 0.01f;
            term.SolverSettings.Relaxation = 1.05f;
            term.SolverSettings.Priority = "Наивысший";
            term.TimeSettings.InitTimeStep = 0.01f; // при охлаждении -- 0.5f
            term.TimeSettings.MaxTimeStep = 100f; // при охлаждении -- 100f
            term.TimeSettings.MinTimeStep = 0.0005f;
        }

        private void WeldingTermDraft(TermalParameters term)
        {
            term.MetallurgicalProcesses = new List<string>() { "Охлаждение", "Нагрев" };
            term.Iterations = 2;
            term.SaveRate = 1;
            term.TermalConvergence.Is_Switched_Tm = true;
            term.TermalConvergence.Tm = 1500;
            term.SolverSettings.Solver = "SOR_iterative";
            term.SolverSettings.MaxIter = 500;
            term.SolverSettings.Precision = 0.01f;
            term.SolverSettings.Relaxation = 1.05f;
            term.SolverSettings.Priority = "Наивысший";
            term.TimeSettings.InitTimeStep = 0.01f; // при охлаждении -- 0.5f
            term.TimeSettings.MaxTimeStep = 0.5f; // при охлаждении -- 100f
            term.TimeSettings.MinTimeStep = 0.0005f;
        }

        private void WeldingMechDraft(MechanicalParameters mech)
        {
            mech.Iterations = 25;
            mech.SaveRate = 5;
            mech.MechanicalConvergence.Is_Physically_NonLinear = true;
            mech.MechanicalConvergence.MaterialPlasticityCoeff = 0.5f;
            mech.MechanicalConvergence.Is_Switched_Um = false;
            mech.MechanicalConvergence.Um = 0.5f;
            mech.MechanicalConvergence.DUm = 0.0005f;
            mech.MechanicalConvergence.SiStm = 1.5f;
            mech.MechanicalConvergence.PlasticityCriterion = 1.25f;
            mech.SolverSettings.Solver = "SOR_iterative";
            mech.SolverSettings.MaxIter = 1000;
            mech.SolverSettings.Precision = 0.0001f;
            mech.SolverSettings.Relaxation = 1.25f;
            mech.SolverSettings.Priority = "Наивысший";
            mech.TimeSettings.InitTimeStep = 0.01f;
            mech.TimeSettings.MaxTimeStep = 0.5f;
            mech.TimeSettings.MinTimeStep = 0.0005f;
        }

        /// <summary>
        /// Принимает внешние и внутрениие данные процесса.
        /// Главный процесс должен быть всегда дольше внутреннего.
        /// После конца внутреннего процесса, главный должен его подхватывать, или продолжаться.
        /// Главный процесс стартует с 0.
        /// Внутри главного процесса не должно быть пустых промежутков (бездействие главного процесса)
        /// В противном случае, падает исключение.
        /// </summary>
        /// <typeparam name="LoadData">Внутренний параметр, вложенный во время выполнения BoundData</typeparam>
        /// <typeparam name="BoundData">Внешний параметр, задающий время выполнения процесса</typeparam>
        /// <param name="loadData">Лист класса LoadData</param>
        /// <param name="boundData">Лист класса BoundData</param>
        /// <exception cref="Exception">Некоректно заданы данные</exception>
        private void CheckLoadDataInsideBoundDataInterval<LoadData, BoundData>
            (List<ICondData> loadData, List<ICondData> boundData)
        {
            if (boundData.Count == 0)
                throw new Exception($"Нет данных среды выполнения {typeof(BoundData)}");

            if (boundData.Min(bd => bd.StartTime) != 0)
                throw new Exception($"{typeof(BoundData)} не началась с началом процесса");

            if ((loadData.Count > 0) && (loadData.Min(id => id.StartTime) < boundData.Min(od => od.StartTime)
                || loadData.Max(id => id.StopTime) > boundData.Max(od => od.StopTime)))
                throw new Exception($"{typeof(LoadData)} вне {typeof(BoundData)} по времени выполнения");

            if (boundData.Count > 1)
            {
                for (var i = 1; i < boundData.Count; i++)
                {

                    if (boundData[i - 1].StopTime < boundData[i].StartTime
                        && !boundData.Any(x => x.StartTime < boundData[i - 1].StopTime && x.StopTime > boundData[i].StartTime))
                        throw new Exception($"Неопределенность: между {typeof(BoundData)} есть пустые промежутки");
                }
            }

            else
            {
                if (loadData.Count == 0)
                    return;

                var maxLoadStop = loadData.Max(id => id.StopTime);

                if (!boundData.Any(bd => bd.StartTime == maxLoadStop)
                    && !(boundData.Max(bd => bd.StopTime) == maxLoadStop)
                    && !boundData.Any(bd => bd.StartTime < maxLoadStop && bd.StopTime > maxLoadStop))
                    throw new Exception($"нагрузка {typeof(LoadData)} заканчивается, но внешний процесс {typeof(BoundData)} не закончился и не продолжился. Создается неопределенность");
            }
        }

        /// <summary>
        /// Создает .tsf-файлы
        /// </summary>
        /// <typeparam name="DataParameter">Тип параметра данных для создания файлов</typeparam>
        /// <param name="taskKind">Тип задачи</param>
        /// <param name="parametrizedParams">Список созданных параметров для создания фалов</param>
        /// <returns>возвращает список строк для создания CompData</returns>
        private void CreateFilesForSingleProcess<DataParameter>(string taskKind, List<DataParameter> parametrizedParams)
            where DataParameter : GeneralParameters
        {
            for (var i = 0; i < parametrizedParams.Count; i++)
            {
                if (i > 0)
                    parametrizedParams[i].RestartFile = $"{taskKind}_{i}_{parametrizedParams[i - 1].TimeSettings.StartTime}_{parametrizedParams[i - 1].TimeSettings.StopTime}.db";

                var serializedData = JsonConvert.SerializeObject(parametrizedParams[i], settingsSerializer);
                var fileName = $"{taskKind}_{i + 1}_{parametrizedParams[i].TimeSettings.StartTime}_{parametrizedParams[i].TimeSettings.StopTime}.tsf";

                File.WriteAllBytes($"{_path}\\{fileName}", Encoding.UTF8.GetBytes(serializedData));
            }
        }

        /// <summary>
        /// Создает файлы для термического процесса
        /// </summary>
        /// <param name="mainParameters">Набор термических параметров</param>
        /// <param name="minorParameters">Набор механических параметров</param>
        /// <returns>Список строк для создания CompData</returns>
        private void CreateFilesForComplexProcess<MainParameter, MinorParameter>(List<MainParameter> mainParameters, List<MinorParameter> minorParameters)
            where MainParameter : GeneralParameters
            where MinorParameter : GeneralParameters, IThermalDependent
        {
            // 0 - main
            // 1 - minor
            var names = processProp.TaskKind.ToString().Replace(" ","").Split(',');
            CreateFilesForSingleProcess(names[0], mainParameters);

            minorParameters[0].ThermalFile = $"{names[0]}_{1}_{minorParameters[0].TimeSettings.StartTime}_{minorParameters[0].TimeSettings.StopTime}.db";

            for (var i = 0; i < minorParameters.Count; i++)
            {
                if (i > 0)
                {
                    minorParameters[i].RestartFile = $"{names[1]}_{i}_{minorParameters[i - 1].TimeSettings.StartTime}_{minorParameters[i - 1].TimeSettings.StopTime}.db";
                    minorParameters[i].ThermalFile = $"{names[0]}_{i + 1}_{minorParameters[i].TimeSettings.StartTime}_{minorParameters[i].TimeSettings.StopTime}.db";
                }
                var serializedData = JsonConvert.SerializeObject(minorParameters[i], settingsSerializer);
                var fileName = $"{names[1]}_{i + 1}_{minorParameters[i].TimeSettings.StartTime}_{minorParameters[i].TimeSettings.StopTime}.tsf";

                File.WriteAllBytes($"{_path}\\{fileName}", Encoding.UTF8.GetBytes(serializedData));
            }
        }

        /// <inheritdoc/>
        /// Регулярное выражение разбивается на 5 сегментов:
        /// 1 - тип задачи из ComplexTaskType. Принимаются раздельные значения из ComplexTaskType
        /// 2 - индекс задачи. Любое число произвольной длины, кроме нуля. Может быть не задано (*)
        /// 3 - время начала задачи. Целое/дробное положительное число. Может быть не задано (*)
        /// 4 - время конца задачи. Целое/дробное положительное число. Может быть не задано (*)
        /// 5 - расширение файла: .tsf
        public List<string> SortCompDataByTimeAndType(string[] dataStrs)
        {
            var sortedLines = new List<string>();

            // Принимаются все доступные значения из Enum ComplexTaskType
            // сложные процессы разбиваются на составляющее и добавляются к допустимым названиям процессов
            // дубликаты убираются
            var names = new List<string>();
            foreach (var name in Enum.GetNames(typeof(TaskKind)))
            {
                if (name.Contains('_')) names.AddRange(name.Split('_'));
                else names.Add(name);
            }
            var res = names.Distinct();

            var available = string.Join("|", res).ToString();
            var pattern = "^((" + available + ")_" +
                "(([1-9])(\\d)*)_" +
                "((\\d{1,})|((\\d{1,}).(\\d{1,5}))|\\*)_" +
                "((\\d{1,})|((\\d{1,}).(\\d{1,5}))|\\*)" +
                "(.tsf))$";

            foreach (var line in dataStrs)
            {
                var file = Path.GetFileName(line);
                if (!Regex.IsMatch(file, pattern))
                    throw new Exception("Имя полученного файла не соответствует шаблону: " +
                    "<тип процесса>_<номер задачи>_<время начала>_<время окончания>.tsf");

                var taskType = file.Split('_')[0];
                sortedLines.Add(string.Join(" ", new string[] { taskType, line, TaskStatus.выполнить.ToString() }));
            }

            return sortedLines.OrderBy(x => int.Parse(Path.GetFileName(x.Split(' ')[1]).Split('_')[1]))
                .ThenByDescending(x => x).ToList();
        }
    }
}
