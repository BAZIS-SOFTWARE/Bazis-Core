using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Хранит стиль отображения одного набора объектов.
    /// </summary>
    public class SetStyle
    {
        /// <summary>
        /// Возвращает явно установленный цвет набора.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Возвращает явно установленный режим отображения набора.
        /// </summary>
        public ViewMode? ViewMode { get; set; }
    }
}
