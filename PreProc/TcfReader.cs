using Newtonsoft.Json;
using Project.Interfaces.Tasks;
using Project.TaskParameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PreProc
{
    /// <summary>Читает очередь задач из TCF и связанных с ним TSF.</summary>
    internal class TcfReader
    {
        /// <summary>Перечисляет параметры в порядке команд TCF со статусами из этих команд.</summary>
        public IEnumerable<GeneralParameters> Read(string projectPath)
        {
            var paths = new CalculationPaths();
            var root = paths.GetProjectRoot(projectPath);
            var projectFile = Path.GetFullPath(projectPath);
            var commandFile = Path.Combine(root, "ComputationData", "computation.tcf");
            var directory = Path.GetDirectoryName(commandFile)!;
            var serializer = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var keys = new HashSet<string>();
            var projectLoaded = false;
            var count = 0;

            foreach (var line in File.ReadLines(commandFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("\\", StringComparison.Ordinal))
                    continue;

                if (line.StartsWith("загрузить", StringComparison.Ordinal))
                {
                    var command = Regex.Match(line, "^загрузить проект \"([^\"]+)\"$");
                    if (!command.Success || projectLoaded)
                        throw new FormatException("Invalid project command in TCF.");
                    var reference = command.Groups[1].Value;
                    if (Path.IsPathRooted(reference) || reference.Contains('\\'))
                        throw new InvalidDataException("The TCF project path must be portable and relative.");
                    var referencedProject = paths.GetAbsoluteFilePath(directory, reference);
                    paths.ValidatePortableFilePath(root, referencedProject);
                    if (!File.Exists(referencedProject))
                        throw new FileNotFoundException("The calculation resource does not exist.", referencedProject);
                    if (!string.Equals(referencedProject, projectFile, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("The TCF refers to a different project.");
                    projectLoaded = true;
                    continue;
                }

                var calculation = Regex.Match(line, "^расчет (\\S+) \"([^\"]+)\" (выполнить|пропустить)$");
                if (!projectLoaded || !calculation.Success)
                    throw new FormatException("Invalid calculation command in TCF.");
                var kindText = calculation.Groups[1].Value;
                if (!Enum.TryParse<TaskKind>(kindText, out var kind) || !Enum.IsDefined(kind) || kindText != kind.ToString())
                    throw new FormatException("Unsupported task kind in TCF.");
                var referencePath = calculation.Groups[2].Value;
                if (Path.IsPathRooted(referencePath) || referencePath.Contains('\\'))
                    throw new InvalidDataException("The TCF task path must be portable and relative.");
                var taskPath = paths.GetAbsoluteFilePath(directory, referencePath);
                paths.ValidatePortableFilePath(root, taskPath);
                if (!File.Exists(taskPath))
                    throw new FileNotFoundException("The calculation resource does not exist.", taskPath);
                var fileKind = Path.GetFileNameWithoutExtension(taskPath).Split('_')[0];
                if (Path.GetExtension(taskPath) != ".tsf" || fileKind != kindText)
                    throw new InvalidDataException("The TCF task kind does not match its TSF file.");

                var json = File.ReadAllText(taskPath);
                GeneralParameters parameters;
                switch (kind)
                {
                    case TaskKind.термическая:
                        parameters = JsonConvert.DeserializeObject<TermalParameters>(json, serializer);
                        break;
                    case TaskKind.механическая:
                        parameters = JsonConvert.DeserializeObject<MechanicalParameters>(json, serializer);
                        break;
                    case TaskKind.химическая:
                        parameters = JsonConvert.DeserializeObject<ChemicalParameters>(json, serializer);
                        break;
                    default:
                        throw new FormatException("Unsupported task kind in TCF.");
                }

                if (parameters == null || parameters.TaskKind != kind || parameters.Index < 1 || parameters.TimeSettings == null || !float.IsFinite(parameters.TimeSettings.StartTime) || !float.IsFinite(parameters.TimeSettings.StopTime) || parameters.TimeSettings.StartTime < 0 || parameters.TimeSettings.StopTime < parameters.TimeSettings.StartTime)
                    throw new InvalidDataException("The TSF contains invalid task parameters.");
                var fileStem = Path.GetFileNameWithoutExtension(taskPath);
                if (fileStem != parameters.FileStem)
                    throw new InvalidDataException("The TSF parameters do not match the task file name.");
                var key = $"{kind}_{parameters.Index}";
                if (!keys.Add(key))
                    throw new InvalidDataException("The TCF contains duplicate task kinds and indices.");
                var statusText = calculation.Groups[3].Value;
                parameters.Status = Enum.Parse<TaskStatus>(statusText);
                count++;
                yield return parameters;
            }

            if (!projectLoaded || count == 0)
                throw new InvalidDataException("The TCF does not contain a project and calculation queue.");
        }
    }
}
