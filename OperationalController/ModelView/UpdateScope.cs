using System;

namespace OperationalController
{
    /// <summary>
    /// Ограничивает время жизни пакета изменений представления.
    /// </summary>
    /// <remarks>
    /// Создаётся методом <see cref="ModelView.BeginUpdate"/>, который уже увеличил
    /// счётчик открытых пакетов. Задача области — вернуть счётчик обратно, и вернуть
    /// его даже при исключении: оператор using разворачивается в try/finally,
    /// поэтому вызывающему нечего забыть. Без этого незакрытый пакет молча
    /// заглушил бы события до конца сеанса.
    /// </remarks>
    internal sealed class UpdateScope : IDisposable
    {
        readonly ModelView view;
        bool isDisposed;

        /// <summary>
        /// Создаёт ограничитель пакета изменений.
        /// </summary>
        public UpdateScope(ModelView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Закрывает пакет. Повторный вызов безвреден: иначе счётчик ушёл бы в минус
        /// и событие поднялось бы посреди чужого пакета.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            view.EndUpdate();
        }
    }
}
