using System;
using System.IO;

namespace PreProc
{
    /// <summary>
    /// Проверяет расположение ресурсов и формирует переносимые ссылки внутри комплекта.
    /// </summary>
    internal class CalculationPaths
    {
        /// <summary>
        /// Проверяет сохранённый проект и возвращает корень расчётного комплекта.
        /// </summary>
        public string GetProjectRoot(string projectPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
            var fullPath = Path.GetFullPath(projectPath);
            var extension = Path.GetExtension(fullPath);
            if (extension != ".bpf" && extension != ".bpf2")
                throw new ArgumentException("A saved .bpf or .bpf2 project is required.", nameof(projectPath));
            var root = Path.GetDirectoryName(fullPath);
            GetRelativeFile(root, root, fullPath, true);
            return root;
        }

        /// <summary>
        /// Проверяет файл в пределах комплекта, точный регистр существующих сегментов
        /// и возвращает ссылку относительно содержащего файла.
        /// </summary>
        public string GetRelativeFile(string root, string sourceDirectory, string targetPath, bool requireExisting)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(targetPath);
            if (targetPath.Contains('\\') && Path.DirectorySeparatorChar != '\\')
                throw new ArgumentException("Backslashes are not supported in portable paths.", nameof(targetPath));
            var absolutePath = Path.GetFullPath(targetPath, sourceDirectory);
            var relativeToRoot = Path.GetRelativePath(root, absolutePath);
            var portable = relativeToRoot.Replace(Path.DirectorySeparatorChar, '/');
            if (Path.IsPathRooted(relativeToRoot) || portable == ".." || portable.StartsWith("../", StringComparison.Ordinal))
                throw new ArgumentException("The resource must be located inside the calculation package.", nameof(targetPath));
            var parts = portable.Split('/');
            foreach (var part in parts)
            {
                if (part.Length == 0 || part == "." || part.EndsWith('.') || part.EndsWith(' '))
                    throw new ArgumentException("The resource path contains a nonportable name.", nameof(targetPath));
                foreach (var character in part)
                {
                    if (character < 32 || "<>:\"\\|?*".Contains(character))
                        throw new ArgumentException("The resource path contains a nonportable character.", nameof(targetPath));
                }
                var stem = part.Split('.')[0].ToUpperInvariant();
                if (stem == "CON" || stem == "PRN" || stem == "AUX" || stem == "NUL" || (stem.Length == 4 && (stem.StartsWith("COM", StringComparison.Ordinal) || stem.StartsWith("LPT", StringComparison.Ordinal)) && stem[3] >= '1' && stem[3] <= '9'))
                    throw new ArgumentException("The resource path contains a reserved file name.", nameof(targetPath));
            }
            var currentDirectory = root;
            foreach (var part in parts)
            {
                if (!Directory.Exists(currentDirectory))
                    break;
                var exactPath = string.Empty;
                var caseMismatch = false;
                var entries = Directory.EnumerateFileSystemEntries(currentDirectory);
                foreach (var entry in entries)
                {
                    var entryName = Path.GetFileName(entry);
                    if (string.Equals(entryName, part, StringComparison.Ordinal))
                    {
                        exactPath = entry;
                        break;
                    }
                    if (string.Equals(entryName, part, StringComparison.OrdinalIgnoreCase))
                        caseMismatch = true;
                }
                if (exactPath.Length == 0)
                {
                    if (caseMismatch)
                        throw new ArgumentException("The resource path does not match the exact case of an existing name.", nameof(targetPath));
                    break;
                }
                currentDirectory = exactPath;
            }
            if (requireExisting && !File.Exists(absolutePath))
                throw new FileNotFoundException("The calculation resource does not exist.", absolutePath);
            var relative = Path.GetRelativePath(sourceDirectory, absolutePath);
            return relative.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}
