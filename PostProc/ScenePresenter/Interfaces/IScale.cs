using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Results.ScenePresenter.Interfaces
{
    public interface IScale
    {
        IEnumerable<Color> ColorRange();

        IEnumerable<float[]> ValueRange();


        float MaxValue { get; }

        float MinValue { get; }

        int Coord_X { get; set; }
        int Coord_Y { get; set; }

        decimal Precision { get; set; }

        Color GetValueColor(float resValue);

        void FillRange(float max, float min, decimal intervals);
    }
}
