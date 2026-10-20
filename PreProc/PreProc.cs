using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PreProc.Interfaces;
using Project.TaskParameters;
using Project.Tasks.Functions;
using Project.Tasks.LocalFrames;
using MaterialDB.MaterialData;

namespace PreProc
{
    /// <summary>
    /// PreProc. Препроцессор
    /// </summary>
    public class PreProc : IPreProc
    {
        private float[] AllTimeSeparators;
        private readonly List<GeneralParameters> currentParameters = new();
        private readonly ISolverInstructionWriter instructionWriter;

        /// <summary>Создаёт препроцессор с форматом инструкций выбранного решателя.</summary>
        public PreProc(ISolverInstructionWriter instructionWriter = null)
        {
            this.instructionWriter = instructionWriter ?? new TcfWriter();
        }

        /// <summary>Перечисляет текущие инструкции в порядке очереди.</summary>
        public IEnumerator<GeneralParameters> GetEnumerator()
        {
            foreach (var parameters in currentParameters)
                yield return parameters;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public void Load(IEnumerable<GeneralParameters> parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            var loaded = parameters.ToList();
            if (loaded.Any(parameter => parameter == null))
                throw new ArgumentException("The calculation queue contains a null parameter.", nameof(parameters));
            currentParameters.Clear();
            currentParameters.AddRange(loaded);
        }

        /// <inheritdoc/>
        public void LoadInstructions(string projectPath)
        {
            var reader = new TcfReader();
            var parameters = reader.Read(projectPath);
            Load(parameters);
        }

        /// <inheritdoc/>
        public void Add(GeneralParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            currentParameters.Add(parameters);
        }

        /// <inheritdoc/>
        public bool Remove(GeneralParameters parameters) => currentParameters.Remove(parameters);

        /// <inheritdoc/>
        public void Clear() => currentParameters.Clear();

        /// <inheritdoc/>
        public string WriteInstructions(string projectPath) => instructionWriter.Write(projectPath, currentParameters);

        /// <inheritdoc/>
        public IReadOnlyList<GeneralParameters> CalcCompDataV2(IEnumerable<ICondData> data, TaskKind taskKinds, bool synchronizeTimeSteps = false)
        {
            ArgumentNullException.ThrowIfNull(data);
            var taskData = data.ToList();
            SetUpTimeSeparators(taskData);
            IEnumerable<GeneralParameters> parameters;
            switch (taskKinds)
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
                    parameters = PrepareComplexProcess(source, target, taskData, synchronizeTimeSteps);
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
            Load(result);
            return currentParameters.AsReadOnly();
        }

        private void SetUpTimeSeparators(IEnumerable<ICondData> valuableData)
        {
            var separators = new List<float>();
            foreach (var data in valuableData)
            {
                separators.Add(data.StartTime);
                separators.Add(data.StopTime);
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

                SetTimeStep<LoadData>(parameter, data, start, stop);
                if (parameter is TermalParameters thermal)
                    SetMetallurgicalProcesses(thermal, data, start, stop);

                result.Add(parameter);
            }

            return result;
        }

        /// <summary>
        /// Выбирает шаг времени по активным движущимся источникам задачи.
        /// </summary>
        private void SetTimeStep<LoadData>(GeneralParameters parameter, IEnumerable<ICondData> data, float start, float stop)
            where LoadData : CondData
        {
            var maxTimeStep = GetMovingSourceTimeStep<LoadData>(data, start, stop) ?? 1f;
            var initialStep = maxTimeStep / 2f;
            if (initialStep < parameter.TimeSettings.MinTimeStep)
                throw new InvalidOperationException("The calculated initial time step is below the minimum time step.");

            parameter.TimeSettings.MaxTimeStep = maxTimeStep;
            parameter.TimeSettings.InitTimeStep = initialStep;
        }

        /// <summary>
        /// Возвращает минимальный допустимый шаг активных движущихся источников задачи.
        /// </summary>
        private float? GetMovingSourceTimeStep<LoadData>(IEnumerable<ICondData> data, float start, float stop)
            where LoadData : CondData
        {
            float? maxTimeStep = null;
            foreach (var condition in data)
            {
                if (condition is not LoadData || condition.StartTime > start || condition.StopTime < stop ||
                    condition.Function is not FrameFunction source || condition.LocalFrame is not MovedFrame frame)
                    continue;

                var sourceStep = source.CalcMaxTimeStep(frame.Velocity);
                if (!float.IsFinite(sourceStep) || sourceStep <= 0)
                    throw new InvalidOperationException("The source time step must be positive and finite.");

                maxTimeStep = maxTimeStep.HasValue ? Math.Min(maxTimeStep.Value, sourceStep) : sourceStep;
            }

            return maxTimeStep;
        }

        /// <summary>
        /// Собирает названия реакций из материалов, действующих на интервале.
        /// </summary>
        private void SetMetallurgicalProcesses(TermalParameters thermal, IEnumerable<ICondData> data, float start, float stop)
        {
            var reactions = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var condition in data)
            {
                if (condition is not MatData matData || matData.StartTime > start || matData.StopTime < stop)
                    continue;

                var material = matData.Material;
                if (material == null)
                    throw new InvalidOperationException("An active material condition has no material.");

                var categoryName = material.CategoryDic[CategoryEnum.Metallurgical];
                if (!material.CategoryData.ContainsKey(categoryName))
                    continue;

                var category = material[CategoryEnum.Metallurgical];
                foreach (var reaction in category.PropertyData.Values)
                    reactions.Add(reaction.Name);
            }

            thermal.MetallurgicalProcesses = reactions.ToList();
        }

        /// <inheritdoc/>
        public void SetDraftParameters<ResultParam>(ResultParam parameter)
            where ResultParam : GeneralParameters, new()
        {
            if (parameter is TermalParameters termalParameter)
                SetThermalDraftParameters(termalParameter);
            else if (parameter is MechanicalParameters mechParameter)
                SetMechanicalDraftParameters(mechParameter);
            else
                throw new NotSupportedException("The physical task has no parameter template.");
        }

        /// <summary>
        /// Задает базовые параметры тепловой задачи.
        /// </summary>
        private void SetThermalDraftParameters(TermalParameters term)
        {
            term.Iterations = 2;
            term.SaveRate = 1;
            term.TermalConvergence.Is_Switched_Tm = true;
            term.TermalConvergence.Tm = 1500;
            term.SolverSettings = new SolverSettings(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
            term.SolverSettings.MaxIter = 500;
            term.SolverSettings.Precision = 0.01f;
            term.SolverSettings.Relaxation = 1.05f;
            term.SolverSettings.Priority = "Наивысший";
            term.TimeSettings.MinTimeStep = 0.0005f;
        }

        /// <summary>
        /// Задает базовые параметры механической задачи.
        /// </summary>
        private void SetMechanicalDraftParameters(MechanicalParameters mech)
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
                    SetInitialFieldFromFile(current, $"../ResultsData/{previous.FileStem}.db");
                }
                yield return current;
            }
        }

        /// <summary>
        /// Подготавливает параметры связанных физических задач.
        /// </summary>
        /// <param name="sourceParameters">Параметры задачи-источника.</param>
        /// <param name="targetParameters">Параметры задачи-потребителя.</param>
        /// <param name="data">Исходные условия расчета.</param>
        /// <param name="synchronizeTimeSteps">Признак общего шага связанных задач.</param>
        private IEnumerable<GeneralParameters> PrepareComplexProcess<SourceParameter, TargetParameter>(List<SourceParameter> sourceParameters, List<TargetParameter> targetParameters, IEnumerable<ICondData> data, bool synchronizeTimeSteps)
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

                if (synchronizeTimeSteps)
                {
                    var start = sourceParameter.TimeSettings.StartTime;
                    var stop = sourceParameter.TimeSettings.StopTime;
                    var sourceStep = GetMovingSourceTimeStep<HeatData>(data, start, stop);
                    var targetStep = GetMovingSourceTimeStep<LoadData>(data, start, stop);
                    var sharedMaxStep = sourceStep.HasValue && targetStep.HasValue
                        ? Math.Min(sourceStep.Value, targetStep.Value)
                        : sourceStep ?? targetStep ?? 1f;
                    var sharedInitialStep = sharedMaxStep / 2f;
                    if (sharedInitialStep < sourceParameter.TimeSettings.MinTimeStep ||
                        sharedInitialStep < targetParameter.TimeSettings.MinTimeStep)
                        throw new InvalidOperationException("The synchronized initial time step is below a task's minimum time step.");

                    sourceParameter.TimeSettings.MaxTimeStep = sharedMaxStep;
                    targetParameter.TimeSettings.MaxTimeStep = sharedMaxStep;
                    sourceParameter.TimeSettings.InitTimeStep = sharedInitialStep;
                    targetParameter.TimeSettings.InitTimeStep = sharedInitialStep;
                }

                if (i > 0)
                {
                    var previousSource = sourceParameters[i - 1];
                    SetInitialFieldFromFile(sourceParameter, $"../ResultsData/{previousSource.FileStem}.db");
                    var previousParameter = targetParameters[i - 1];
                    SetInitialFieldFromFile(targetParameter, $"../ResultsData/{previousParameter.FileStem}.db");
                }
                SetInputField(targetParameter, sourceFieldSet, $"../ResultsData/{sourceParameter.FileStem}.db");
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

    }
}
