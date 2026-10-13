using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Определяет способ задания цветов объектов.
    /// </summary>
    public enum ObjsColorsType
    {
        /// <summary>
        /// Один цвет для всех объектов.
        /// </summary>
        Single,

        /// <summary>
        /// Индивидуальный цвет для каждого объекта.
        /// </summary>
        Many
    }

    /// <summary>
    /// Создаёт последовательность цветов отображаемых объектов.
    /// </summary>
    public interface IObjsColors
    {
        /// <summary>
        /// Способ задания цветов объектов.
        /// </summary>
        ObjsColorsType Type { get; }

        /// <summary>
        /// Создаёт последовательность цветов для заданного количества объектов.
        /// </summary>
        IEnumerable<Color> CreateColors(int objectsCount);
    }

    /// <summary>
    /// Создаёт один цвет для всех отображаемых объектов.
    /// </summary>
    public class SingleColorObjsColors : IObjsColors
    {
        private readonly Color color;

        /// <summary>
        /// Создаёт источник одного цвета для всех объектов.
        /// </summary>
        public SingleColorObjsColors(Color color)
        {
            this.color = color;
        }

        /// <inheritdoc/>
        public ObjsColorsType Type => ObjsColorsType.Single;

        /// <inheritdoc/>
        public IEnumerable<Color> CreateColors(int objectsCount)
        {
            for (var index = 0; index < objectsCount; index++)
                yield return color;
        }
    }

    /// <summary>
    /// Создаёт индивидуальные цвета отображаемых объектов.
    /// </summary>
    public class ManyColorsObjsColors : IObjsColors
    {
        private readonly IReadOnlyList<Color> colors;

        /// <summary>
        /// Создаёт источник индивидуальных цветов объектов.
        /// </summary>
        public ManyColorsObjsColors(IReadOnlyList<Color> colors)
        {
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));
        }

        /// <inheritdoc/>
        public ObjsColorsType Type => ObjsColorsType.Many;

        /// <inheritdoc/>
        public IEnumerable<Color> CreateColors(int objectsCount)
        {
            if (colors.Count != objectsCount)
                throw new ArgumentException("The number of colors must match the number of objects.", nameof(objectsCount));

            foreach (var color in colors)
                yield return color;
        }
    }
}
