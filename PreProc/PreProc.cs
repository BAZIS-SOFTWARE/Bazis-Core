using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
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
        private Dictionary<DataKind, List<Tuple<float, float>>> TimeSeparators = new Dictionary<DataKind, List<Tuple<float, float>>>();

        private float[] AllTimeSeparators;
        private ProcessProperty processProp;

        /// <inheritdoc/>
        public string WriteCommandFile(string projectPath, IEnumerable<string> queueRows)
        {
            var writer = new TcfWriter();
            return writer.Write(projectPath, queueRows);
        }

        /// <inheritdoc/>
        public IReadOnlyList<GeneralParameters> CalcCompDataV2(IEnumerable<ICondData> data, ProcessProperty property)
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(property);
            var taskData = data.ToList();
            SetUpTimeSeparators(taskData);
            processProp = property;
            IEnumerable<GeneralParameters> parameters;
            switch (property.TaskKind)
            {
                case TaskKind.термическая:
                    var thermal = GetParameters<HeatData, MediaData, TermalParameters>(taskData);
                    parameters = PrepareSingleProcess(thermal);
                    break;

                case TaskKind.механическая:
                    var mechanical = GetParameters<LoadData, ClampData, MechanicalParameters>(taskData);
                    parameters = PrepareSingleProcess(mechanical);
                    break;

                case TaskKind.термическая | TaskKind.механическая:
                    var source = GetParameters<HeatData, MediaData, TermalParameters>(taskData);
                    var target = GetParameters<LoadData, ClampData, MechanicalParameters>(taskData);
                    parameters = PrepareComplexProcess(source, target);
                    break;

                default:
                    throw new NotImplementedException();
            }
            var result = parameters.ToList();
            foreach (var item in result)
            {
                if (!Enum.IsDefined(item.TaskKind) || item.Index < 1 || !float.IsFinite(item.TimeSettings.StartTime) || !float.IsFinite(item.TimeSettings.StopTime) || item.TimeSettings.StartTime < 0 || item.TimeSettings.StopTime < item.TimeSettings.StartTime)
                    throw new ArgumentException("The calculated task kind, index or time interval is invalid.", nameof(data));
            }
            return result.AsReadOnly();
        }

        private void SetUpTimeSeparators(IEnumerable<ICondData> valuableData)
        {
            var separators = new List<float>();
            TimeSeparators.Clear();

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

            var pars = ParametrizeResultParameters<LoadData, BoundData, ResultParam>(data);

            var initialField = new PhysicalSet
            {
                Name = GetNativeFieldSet(pars[0].TaskKind),
                Source = PhysicalSetSource.Values,
                Values = matData
                    .Select(md => md.Group.Name)
                    .Distinct()
                    .ToDictionary(groupName => groupName, _ => new List<PhysicalField>
                    {
                        new PhysicalField
                        {
                            Name = PhysicalFieldName.Temperature,
                            Components = new[] { 20.0 }
                        }
                    })
            };

            pars[0].InitialSet = initialField;

            return pars;
        }

        /// <summary>
        /// Параметризирует временные свойства созданного по dataItem ResultParam
        /// Задает значения ResultParam по шаблону
        /// </summary>
        /// <typeparam name="LoadData">Тип нагрузки задачи.</typeparam>
        /// <typeparam name="BoundData">Тип граничного условия задачи.</typeparam>
        /// <typeparam name="ResultParam">Параметр, созданный с помощью dataItem</typeparam>
        /// <param name="data">Полный упорядоченный список условий TaskData.</param>
        /// <returns>Список параметров по шаблону с указанным временем действия</returns>
        private List<ResultParam> ParametrizeResultParameters<LoadData, BoundData, ResultParam>(IEnumerable<ICondData> data)
            where ResultParam : GeneralParameters, new()
            where LoadData : CondData
            where BoundData : CondData
        {
            var result = new List<ResultParam>();
            for (var i = 0; i < AllTimeSeparators.Length - 1; i++)
            {
                var start = AllTimeSeparators[i];
                var stop = AllTimeSeparators[i + 1];

                var parameter = new ResultParam();
                SetDraftParameters(parameter);
                parameter.TimeSettings.StartTime = start;
                parameter.TimeSettings.StopTime = stop;
                parameter.ActiveConditions = data
                    .Select((item, index) => new { Item = item, Index = index })
                    .Where(item =>
                        item.Item.StartTime <= start && item.Item.StopTime >= stop)
                    .Select(item => new ConditionReference(item.Index, item.Item.Kind, item.Item.Group.Name))
                    .ToList();

                if (TimeSeparators.ContainsKey(DataKind.Нагрев))
                    if (!TimeSeparators[DataKind.Нагрев].Any(x => x.Item1 == start && x.Item2 == stop))
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
            mech.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
            term.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
            term.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
            term.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
            term.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
            mech.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
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
        /// Подготавливает параметры последовательных задач одной физики.
        /// </summary>
        /// <typeparam name="DataParameter">Тип параметров физической задачи.</typeparam>
        /// <param name="parametrizedParams">Параметры по временным интервалам.</param>
        /// <returns>Параметры с индексами и ссылками на предыдущие результаты.</returns>
        private IEnumerable<GeneralParameters> PrepareSingleProcess<DataParameter>(List<DataParameter> parametrizedParams)
            where DataParameter : GeneralParameters
        {
            for (var i = 0; i < parametrizedParams.Count; i++)
            {
                var current = parametrizedParams[i];
                current.Index = i + 1;
                if (i > 0)
                {
                    var previous = parametrizedParams[i - 1];
                    SetInitialFieldFromFile(current, $"../ResultsData/{previous}.db");
                }
                yield return current;
            }
        }

        /// <summary>
        /// Подготавливает параметры связанных физических задач.
        /// </summary>
        /// <param name="sourceParameters">Параметры задачи-источника.</param>
        /// <param name="targetParameters">Параметры задачи-потребителя.</param>
        private IEnumerable<GeneralParameters> PrepareComplexProcess<SourceParameter, TargetParameter>(List<SourceParameter> sourceParameters, List<TargetParameter> targetParameters)
            where SourceParameter : GeneralParameters
            where TargetParameter : GeneralParameters
        {
            if (sourceParameters.Count != targetParameters.Count)
                throw new InvalidOperationException("Linked tasks must have the same number of time intervals.");
            if (sourceParameters.Count == 0)
                throw new InvalidOperationException("Linked tasks must contain at least one time interval.");

            var sourceFieldSet = GetNativeFieldSet(sourceParameters[0].TaskKind);

            for (var i = 0; i < targetParameters.Count; i++)
            {
                var sourceParameter = sourceParameters[i];
                var targetParameter = targetParameters[i];
                sourceParameter.Index = i + 1;
                targetParameter.Index = i + 1;

                if (i > 0)
                {
                    var previousSource = sourceParameters[i - 1];
                    SetInitialFieldFromFile(sourceParameter, $"../ResultsData/{previousSource}.db");
                    var previousParameter = targetParameters[i - 1];
                    SetInitialFieldFromFile(targetParameter, $"../ResultsData/{previousParameter}.db");
                }
                SetInputField(targetParameter, sourceFieldSet, $"../ResultsData/{sourceParameter}.db");
                yield return sourceParameter;
                yield return targetParameter;
            }
        }

        /// <summary>
        /// Задает родной набор полей из результата предыдущей задачи.
        /// </summary>
        private void SetInitialFieldFromFile(GeneralParameters parameters, string fileName)
        {
            var fieldSet = GetNativeFieldSet(parameters.TaskKind);
            var field = parameters.InitialSet;
            if (field == null)
            {
                field = new PhysicalSet { Name = fieldSet };
                parameters.InitialSet = field;
            }
            else if (field.Name != fieldSet)
                throw new InvalidOperationException($"The initial field set {field.Name} does not match the task field set {fieldSet}.");

            field.Source = PhysicalSetSource.ResultFile;
            field.Values.Clear();
            field.File = fileName;
        }

        /// <summary>
        /// Задает внешний набор полей из результата другой задачи.
        /// </summary>
        private void SetInputField(GeneralParameters parameters, PhysicalSetName fieldSet, string fileName)
        {
            parameters.InputSets ??= new List<PhysicalSet>();
            var field = parameters.GetInputField(fieldSet);
            if (field == null)
            {
                field = new PhysicalSet { Name = fieldSet };
                parameters.InputSets.Add(field);
            }

            field.Source = PhysicalSetSource.ResultFile;
            field.Values.Clear();
            field.File = fileName;
        }

        /// <summary>
        /// Возвращает родной набор полей для физической задачи.
        /// </summary>
        private PhysicalSetName GetNativeFieldSet(TaskKind taskKind)
        {
            return taskKind switch
            {
                TaskKind.термическая => PhysicalSetName.Thermal,
                TaskKind.механическая => PhysicalSetName.Mechanical,
                TaskKind.химическая => PhysicalSetName.Chemical,
                _ => throw new ArgumentOutOfRangeException(nameof(taskKind), taskKind, "The task kind does not have a native field set.")
            };
        }

        /// <inheritdoc/>
        public List<string> SortCompDataByTimeAndType(string[] dataStrs)
        {
            ArgumentNullException.ThrowIfNull(dataStrs);
            var indices = new Dictionary<string, int>();
            var kinds = new Dictionary<string, TaskKind>();
            var keys = new HashSet<string>();
            foreach (var line in dataStrs)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(line);
                var file = Path.GetFileName(line);
                var parts = Path.GetFileNameWithoutExtension(file).Split('_');
                if (Path.GetExtension(file) != ".tsf" || parts.Length != 4)
                    throw new DraftException("The instruction file name is invalid.", new ArgumentException("The task file name is invalid.", nameof(dataStrs)));
                var validKind = Enum.TryParse<TaskKind>(parts[0], out var kind) && Enum.IsDefined(kind) && parts[0] == kind.ToString();
                var validIndex = int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var index) && index > 0;
                var validStart = float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var start) && float.IsFinite(start) && start >= 0;
                var validStop = float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var stop) && float.IsFinite(stop) && stop >= start;
                if (!validKind || !validIndex || !validStart || !validStop)
                    throw new DraftException("The instruction file name is invalid.", new ArgumentException("The task file name is invalid.", nameof(dataStrs)));
                var key = $"{kind}_{index}";
                if (!keys.Add(key))
                    throw new DraftException("The instruction file name is invalid.", new ArgumentException("The task kind and index are duplicated.", nameof(dataStrs)));
                indices.Add(line, index);
                kinds.Add(line, kind);
            }
            var instructions = dataStrs.OrderBy(line => indices[line]).ThenBy(line => kinds[line] switch
            {
                TaskKind.термическая => 0,
                TaskKind.механическая => 1,
                _ => 2
            });
            var sortedLines = new List<string>();
            foreach (var line in instructions)
                sortedLines.Add($"{kinds[line]} {line} {TaskStatus.выполнить}");
            return sortedLines;
        }
    }
}
