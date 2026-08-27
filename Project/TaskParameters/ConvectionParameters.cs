using System;

namespace Project.TaskParameters
{
    /// <summary>Параметры конвективного переноса.</summary>
    [Serializable]
    public class ConvectionParameters
    {
        /// <summary>Использовать файл результатов поля скоростей.</summary>
        public bool ConvectionIsFile { get; set; }

        /// <summary>Путь к файлу результатов поля скоростей.</summary>
        public string ConvectionFile { get; set; } = "";

        /// <summary>Компоненты постоянной скорости конвекции: x, y, z.</summary>
        public double[] ConvectionlLoad { get; set; } = [];

        /// <summary>Учитывать конвективный перенос.</summary>
        public bool Convection { get; set; }
    }
}
