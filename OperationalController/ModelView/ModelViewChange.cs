using System;

namespace OperationalController
{
    /// <summary>
    /// Вид изменения состояния представления.
    /// </summary>
    [Flags]
    public enum ModelViewChange
    {
        /// <summary>Изменений нет.</summary>
        None = 0,

        /// <summary>Изменилось выделение.</summary>
        Selection = 1,

        /// <summary>Изменилась видимость.</summary>
        Visibility = 2,

        /// <summary>Изменился цвет набора.</summary>
        SetColor = 4,

        /// <summary>Изменился режим отображения набора.</summary>
        ViewMode = 8,

        /// <summary>Изменился цвет выделения.</summary>
        SelectionColor = 16,

        /// <summary>Изменилась прозрачность.</summary>
        Transparency = 32,

        /// <summary>Изменился признак скрытия внутренних граней.</summary>
        InsideSurfaces = 64,

        /// <summary>Изменился индивидуальный цвет объекта.</summary>
        ObjectColor = 128
    }
}
