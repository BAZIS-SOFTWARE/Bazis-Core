using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Project.Interfaces.Tasks;

namespace PreProc
{
    /// <summary>
    /// Записывает TCF из актуальной очереди без изменения её порядка и файлов TSF.
    /// </summary>
    public class TcfWriter
    {
        /// <summary>
        /// Проверяет очередь и записывает ComputationData/computation.tcf в UTF-8 без BOM.
        /// </summary>
        public string Write(string projectPath, IEnumerable<string> queueRows)
        {
            ArgumentNullException.ThrowIfNull(queueRows);
            var paths = new CalculationPaths();
            var root = paths.GetProjectRoot(projectPath);
            var projectFile = Path.GetFullPath(projectPath);
            var directory = Path.Combine(root, "ComputationData");
            var projectReference = paths.GetRelativeFile(root, directory, projectFile, true);
            var lines = new List<string> { $"загрузить проект \"{projectReference}\"" };
            var keys = new HashSet<string>();
            foreach (var row in queueRows)
                lines.Add(CreateTaskCommand(row, root, directory, paths, keys));

            if (keys.Count == 0)
                throw new ArgumentException("The calculation queue must not be empty.", nameof(queueRows));

            var result = Path.Combine(directory, "computation.tcf");
            paths.GetRelativeFile(root, root, result, false);
            ReplaceCommandFile(directory, result, lines);
            return result;
        }

        /// <summary>
        /// Проверяет строку очереди и формирует команду расчёта с путём относительно TCF.
        /// </summary>
        private string CreateTaskCommand(string row, string root, string directory, CalculationPaths paths, HashSet<string> keys)
        {
            // Строка очереди должна содержать данные задачи, а не быть пустой.
            if (string.IsNullOrWhiteSpace(row))
                throw new ArgumentException("The calculation queue contains an empty row.", "queueRows");
            var firstSeparator = row.IndexOf(' ');
            var lastSeparator = row.LastIndexOf(' ');
            // Первое и последнее поля отделяют вид задачи и статус от пути к TSF.
            if (firstSeparator < 1 || lastSeparator <= firstSeparator + 1 || lastSeparator == row.Length - 1)
                throw new ArgumentException("The calculation queue row must contain a task kind, path and status.", "queueRows");
            var kindText = row[..firstSeparator];
            var taskFile = row[(firstSeparator + 1)..lastSeparator].Trim();
            var statusText = row[(lastSeparator + 1)..];
            // Вид задачи должен точно совпадать с именем поддерживаемого значения перечисления.
            if (!Enum.TryParse<TaskKind>(kindText, out var kind) || !Enum.IsDefined(kind) || kindText != kind.ToString())
                throw new ArgumentException("The calculation queue contains an unsupported task kind.", "queueRows");
            // Статус допускает только значения, определённые в TaskStatus.
            if (!Enum.TryParse<TaskStatus>(statusText, out var status) || !Enum.IsDefined(status) || statusText != status.ToString())
                throw new ArgumentException("The calculation queue contains an unsupported status.", "queueRows");
            // Путь из GUI должен быть относительным и использовать переносимые разделители.
            if (taskFile.Length == 0 || Path.IsPathRooted(taskFile) || taskFile.IndexOfAny(new[] { '\\', ':', '"', '\r', '\n' }) >= 0)
                throw new ArgumentException("The task file must be a portable relative path.", "queueRows");
            var file = Path.GetFileName(taskFile);
            var parts = Path.GetFileNameWithoutExtension(file).Split('_');
            // Имя TSF должно состоять из вида задачи, индекса и границ временного интервала.
            if (Path.GetExtension(file) != ".tsf" || parts.Length != 4)
                throw new ArgumentException("The task file name does not match the calculation queue.", "queueRows");
            var validIndex = int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var index) && index > 0;
            var validStart = float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var start) && float.IsFinite(start) && start >= 0;
            var validStop = float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var stop) && float.IsFinite(stop) && stop >= start;
            // Вид в имени должен совпадать со строкой очереди; индекс и время должны быть допустимыми.
            if (parts[0] != kindText || !validIndex || !validStart || !validStop)
                throw new ArgumentException("The task file name does not match the calculation queue.", "queueRows");
            var key = $"{kind}_{index.ToString(CultureInfo.InvariantCulture)}";
            // Пара «вид задачи — индекс» не должна повторяться в очереди.
            if (!keys.Add(key))
                throw new ArgumentException("The calculation queue contains duplicate task kinds and indices.", "queueRows");
            var absoluteFile = Path.GetFullPath(taskFile, root);
            // TSF должен существовать внутри комплекта; проверяются также имена сегментов и их регистр.
            var taskReference = paths.GetRelativeFile(root, directory, absoluteFile, true);
            return $"расчет {kind} \"{taskReference}\" {status}";
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
