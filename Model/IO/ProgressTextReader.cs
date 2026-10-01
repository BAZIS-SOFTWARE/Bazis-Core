using System;
using System.IO;
using System.Linq;

namespace Model.IO
{
    /// <summary>
    /// Считает прочитанные строки текстового файла и сообщает их долю.
    /// </summary>
    public sealed class ProgressTextReader : StreamReader
    {
        private readonly IProgress<int> progress;
        private readonly long totalLines;
        private long readLines;
        private int lastPercent = -1;

        /// <summary>
        /// Открывает файл и определяет общее число строк для расчёта процента.
        /// </summary>
        public ProgressTextReader(string path, IProgress<int> progress) : base(path)
        {
            this.progress = progress;
            if (progress == null)
                return;

            totalLines = File.ReadLines(path).LongCount();
            progress.Report(0);
            if (totalLines == 0)
                progress.Report(100);
        }

        /// <summary>
        /// Читает строку и обновляет процент прочитанных строк.
        /// </summary>
        public override string ReadLine()
        {
            var line = base.ReadLine();
            if (line == null || progress == null)
                return line;

            readLines++;
            var percent = totalLines == 0 ? 100 : (int)(100 * readLines / totalLines);
            if (percent != lastPercent)
            {
                progress.Report(percent);
                lastPercent = percent;
            }
            return line;
        }
    }
}
