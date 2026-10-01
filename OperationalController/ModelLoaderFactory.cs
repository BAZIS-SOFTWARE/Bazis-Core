using Model.Interfaces;
using Model.IO;
using Model.IO.STL;
using System;

namespace OperationalController
{
    /// <summary>
    /// Выбирает загрузчик сеточной модели по расширению файла.
    /// </summary>
    internal sealed class ModelLoaderFactory
    {
        /// <summary>
        /// Создаёт загрузчик поддерживаемого формата.
        /// </summary>
        public IModelLoader Create(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            return extension.ToLowerInvariant() switch
            {
                ".inp" => new LoadModelFromINPTextFile(),
                ".inp_v2" => new LoadModelFromINPTextFile_v2(),
                ".asc" => new LoadModelFromASCIITextFile_v2(),
                ".dat" => new LoadModelFromSalomeFile(),
                ".stl" => new LoadFromSTLFile(),
                ".cdb" => new LoadModelFromCDBTextFile(),
                ".bpf" => new LoadModelFromBPFTextFile(),
                ".bpf2" => new LoadModelFromBPF2TextFile(),
                _ => throw new NotSupportedException($"Unsupported model format: {extension}.")
            };
        }
    }
}
