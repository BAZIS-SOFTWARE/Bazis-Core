using Functions.Search;
using Results.ScenePresenter.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Results.ScenePresenter
{
    public class RainbowScale : IScale
    {
        Dictionary<Color, float[]> range;

        public IEnumerable<Color> ColorRange()
        {
            foreach (var color in range.Keys)
            {
                yield return color;
            }
        }

        public IEnumerable<float[]> ValueRange()
        {
            foreach (var range in range.Values)
            {
                yield return range;
            }
        }

        public float MaxValue
        {
            get
            {
                return range.Values.Last()[1];
            }
        }

        public float MinValue
        {
            get
            {
                return range.Values.First()[0];
            }
        }

        public int Coord_X { get; set; } = 70;
        public int Coord_Y { get; set; } = 170;

        public decimal Precision { get; set; } = 4;

        public Color GetValueColor(float resValue)
        {
            var values = range.Values.ToList();
            var colorInd = LinearSearch.CloseLinear(values, resValue);

            var colors = range.Keys.ToList();

            if (colorInd < 0) colorInd = 0;
            return colors[colorInd];
        }


        public RainbowScale()
        {            
            FillRange(1, 0, 10);
        }

        public RainbowScale(float max, float min, decimal intervals)
        {
            FillRange(max, min, intervals);
        }

        public void FillRange(float max, float min, decimal intervals)
        {
            range = new Dictionary<Color, float[]>();
            var values = CreateValueRange(max, min, intervals);

            var colors = CreateColorRange(intervals);

            for (int i = 0; i < colors.Length; i++)
            {
                range.Add(colors[i], values[i]);
            }
        }

        private Color[] CreateColorRange(decimal intervals)
        {
            switch (intervals)
            {
                case 2:
                    return new Color[] 
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"), 
                        System.Drawing.ColorTranslator.FromHtml("#ff0000") 
                    };
                case 3:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff3c"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 4:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00fff7"),
                        System.Drawing.ColorTranslator.FromHtml("#84ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 5:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00aaff"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff3c"),
                        System.Drawing.ColorTranslator.FromHtml("#e1ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 6:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#006fff"),
                        System.Drawing.ColorTranslator.FromHtml("#00ffaa"),
                        System.Drawing.ColorTranslator.FromHtml("#37ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ffe100"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 7:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#0048ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00fff7"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff3c"),
                        System.Drawing.ColorTranslator.FromHtml("#84ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ffbf00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 8:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#002fff"),
                        System.Drawing.ColorTranslator.FromHtml("#00d0ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff8c"),
                        System.Drawing.ColorTranslator.FromHtml("#1aff00"),
                        System.Drawing.ColorTranslator.FromHtml("#bbff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ffa200"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                case 9:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#001aff"),
                        System.Drawing.ColorTranslator.FromHtml("#00aaff"),
                        System.Drawing.ColorTranslator.FromHtml("#00ffc8"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff3c"),
                        System.Drawing.ColorTranslator.FromHtml("#55ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#e1ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff8c00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
                default:
                    return new Color[]
                    {
                        System.Drawing.ColorTranslator.FromHtml("#7300ff"),
                        System.Drawing.ColorTranslator.FromHtml("#000dff"),
                        System.Drawing.ColorTranslator.FromHtml("#0088ff"),
                        System.Drawing.ColorTranslator.FromHtml("#00fff7"),
                        System.Drawing.ColorTranslator.FromHtml("#00ff77"),
                        System.Drawing.ColorTranslator.FromHtml("#04ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#84ff00"),
                        System.Drawing.ColorTranslator.FromHtml("#fffb00"),
                        System.Drawing.ColorTranslator.FromHtml("#ff8000"),
                        System.Drawing.ColorTranslator.FromHtml("#ff0000")
                    };
            }
        }

        private List<float[]> CreateValueRange(float max, float min, decimal intervals)
        {
            var range = GenerateValueRanges(max, min, intervals);
            RoundValueRanges(range);

            return range;
        }

        private void RoundValueRanges(List<float[]> range)
        {
            foreach (var rangeItem in range)
            {
                var tempItem = rangeItem.Select(y => y = (float)Math.Round(y, (int)Precision)).ToArray();
                rangeItem[0] = tempItem[0];
                rangeItem[1] = tempItem[1];
            }
        }

        private static List<float[]> GenerateValueRanges(float max, float min, decimal intervals)
        {
            var range = new List<float[]>();

            var step = (max - min) / (float)intervals;
            //var step = 1400;
            var value = min;

            for (int i = 0; i < intervals; i++)
            {
                var subrange = new float[2];
                for (int j = 0; j < subrange.Length; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        subrange[j] = min;
                    }
                    else if (i == range.Count - 1 && j == range[i].Length - 1)
                    {
                        subrange[j] = max;
                    }
                    else
                    {
                        if (j == 0)
                        {
                            subrange[j] = range[i - 1][1];
                        }
                        else
                        {
                            value = value + step;
                            subrange[j] = value;
                        }
                    }
                }
                range.Add(subrange);
            }

            return range;
        }
    }
}
