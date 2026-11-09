namespace MaterialDB.Validation
{
    /// <summary>
    /// Уровень замечания проверки.
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>
        /// Данные нельзя использовать в расчёте.
        /// </summary>
        Error,
        /// <summary>
        /// Данные допустимы, но часть их будет проигнорирована или вызывает сомнение.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Замечание проверки материала или базы функций.
    /// </summary>
    public sealed class ValidationIssue
    {
        /// <summary>
        /// Создаёт замечание.
        /// </summary>
        public ValidationIssue(IssueSeverity severity, string source, string category, string item, string message)
        {
            Severity = severity;
            Source = source;
            Category = category;
            Item = item;
            Message = message;
        }

        public IssueSeverity Severity { get; }

        /// <summary>
        /// Имя материала или базы функций.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Категория материала; для функций — пустая строка.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Имя записи DataItem; пустая строка для замечаний ко всему материалу.
        /// </summary>
        public string Item { get; }

        public string Message { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Severity}: {Source} / {Category} / {Item}: {Message}";
        }
    }
}
