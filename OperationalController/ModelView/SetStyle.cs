using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Хранит стиль отображения одного набора объектов.
    /// </summary>
    public class SetStyle
    {
        /// <summary>
        /// Создаёт стиль с заданными цветом и режимом отображения.
        /// </summary>
        public SetStyle(Color color, ViewMode viewMode)
        {
            Color = color;
            ViewMode = viewMode;
        }

        /// <summary>
        /// Возвращает цвет набора.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Возвращает режим отображения набора.
        /// </summary>
        public ViewMode ViewMode { get; set; }
    }
}
