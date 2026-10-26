using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PreProc.Interfaces;
using Project.TaskParameters;

namespace PreProc
{
    /// <summary>
    /// Записывает TCF из актуальной очереди задач.
    /// </summary>
    public class TcfWriter : ISolverInstructionWriter
    {
        /// <summary>Записывает TSF для всех задач и формирует TCF с их статусами.</summary>
        public string Write(string projectPath, IEnumerable<GeneralParameters> parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            var paths = new CalculationPaths();
            var root = paths.GetProjectRoot(projectPath);
            var projectFile = Path.GetFullPath(projectPath);
            var inputDirectory = Path.Combine(root, "InputData");
            var directory = Path.Combine(root, "ComputationData");
            var projectReference = paths.GetRelativeFile(directory, projectFile);
            var files = new Dictionary<string, string>();
            var lines = new List<string> { $"загрузить проект \"{projectReference}\"" };
            var keys = new HashSet<string>();
            var serializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            foreach (var parameter in parameters)
            {
                if (parameter == null || !Enum.IsDefined(parameter.Status))
                    throw new ArgumentException("The calculation queue contains invalid parameters.", nameof(parameters));
                if (!Enum.IsDefined(parameter.TaskKind) || parameter.Index < 0 || parameter.TimeSettings == null || !float.IsFinite(parameter.TimeSettings.StartTime) || !float.IsFinite(parameter.TimeSettings.StopTime) || parameter.TimeSettings.StartTime < 0 || parameter.TimeSettings.StopTime < parameter.TimeSettings.StartTime)
                    throw new ArgumentException("The calculation queue contains an invalid task.", nameof(parameters));
                var key = $"{parameter.TaskKind}_{parameter.Index.ToString(CultureInfo.InvariantCulture)}";
                if (!keys.Add(key))
                    throw new ArgumentException("The calculation queue contains duplicate task kinds and indices.", nameof(parameters));

                var file = Path.Combine(inputDirectory, $"{parameter.FileStem}.tsf");
                paths.ValidatePortableFilePath(root, file);
                var relative = paths.GetRelativeFile(directory, file);
                if (!files.TryAdd(file, JsonConvert.SerializeObject(parameter, serializer)))
                    throw new ArgumentException("The calculation queue contains duplicate task files.", nameof(parameters));
                lines.Add($"расчет {parameter.TaskKind} \"{relative}\" {parameter.Status}");
            }

            if (files.Count == 0)
                throw new ArgumentException("The calculation queue must not be empty.", nameof(parameters));

            var result = Path.Combine(directory, "computation.tcf");
            paths.ValidatePortableFilePath(root, result);
            Directory.CreateDirectory(inputDirectory);
            foreach (var file in files)
                File.WriteAllText(file.Key, file.Value, new UTF8Encoding(false));

            ReplaceCommandFile(directory, result, lines);
            return result;
        }

        /// <summary>
        /// Сначала полностью записывает новый TCF во временный файл, затем заменяет им целевой.
        /// Если запись не удалась, прежний TCF остаётся без изменений; временный файл удаляется.
        /// </summary>
        private void ReplaceCommandFile(string directory, string result, List<string> lines)
        {
            Directory.CreateDirectory(directory);
            var temporaryName = $".computation.{Guid.NewGuid():N}.tmp";
            var temporary = Path.Combine(directory, temporaryName);
            try
            {
                var encoding = new UTF8Encoding(false);
                File.WriteAllLines(temporary, lines, encoding);
                File.Move(temporary, result, true);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }
    }
}
