using System;

namespace Project.IO
{
    /// <summary>
    /// Преобразует процент одного прохода чтения в общую шкалу проекта.
    /// </summary>
    internal sealed class ProgressRange : IProgress<int>
    {
        private readonly IProgress<int> progress;
        private readonly int start;
        private readonly int length;

        /// <summary>
        /// Создаёт диапазон процентов для одного прохода чтения.
        /// </summary>
        public ProgressRange(IProgress<int> progress, int start, int length)
        {
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
            this.start = start;
            this.length = length;
        }

        /// <summary>
        /// Передаёт общий процент после преобразования локального.
        /// </summary>
        public void Report(int value)
        {
            progress.Report(start + length * Math.Clamp(value, 0, 100) / 100);
        }
    }
}
