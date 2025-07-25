using PostProc.IO;
using PostProc.ScenePresenter;
using PostProc.ScenePresenter.Interfaces;
using Project.Interfaces.Results;
using Project.Results.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PostProc
{
    /// <summary>
    /// ResultsController
    /// </summary>
    public class PostProcController
    {
        private ItemRange[] items;
        /// <summary>
        /// Intervals
        /// </summary>
        public int Intervals
        {
            get { return items.Length; }
        }
        /// <summary>
        /// MaxValue
        /// </summary>
        public float MaxValue
        {
            get
            {
                return items.Last().Max;
            }
        }
        /// <summary>
        /// MinValue
        /// </summary>
        public float MinValue
        {
            get
            {
                return items.First().Min;
            }
        }
        /// <summary>
        /// GetItems
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ItemRange> GetItems()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }


        /// <summary>
        /// ResultsMerger
        /// </summary>
        public MergeResults ResultsMerger { get; } = new MergeResults();

        /// <summary>
        /// ResultsFieldsCreator
        /// </summary>
        public ResultsFieldsCreator ResultsFieldsCreator { get; } = new ResultsFieldsCreator();

        /// <summary>
        /// ResultsExporter
        /// </summary>
        public IResultsExporter ResultsExporter { get; }  = new ResultsExporter();

        /// <summary>
        /// GridExporter
        /// </summary>
        public IGridExporter GridExporter { get; } = new GridExporter();
        /// <summary>
        /// FillRange
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="intervals"></param>
        /// <param name="precision"></param>
        public void FillRange(float min, float max, int intervals, int precision)
        {
            if (intervals < 2)
                throw new Exception("Должно быть минимум два интервалов!");
            if (intervals > 10)
                throw new Exception("Допускается не больше 10 интервалов");
            
            items = new ItemRange[intervals];

            var values = GenerateValueRanges(min, max, intervals);
            RoundValueRanges(values, precision);
            var colors = CreateColorRange(intervals);

            for (int i = 0; i < colors.Length; i++)
            {
                items[i] = new ItemRange() { Color = colors[i], Min = values[i][0], Max = values[i][1] };
            }
        }

        private List<float[]> GenerateValueRanges(float min, float max, int intervals)
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

        private Color[] CreateColorRange(decimal intervals)
        {
            switch (intervals)
            {
                case 2:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(255, 0, 0),
                    };
                case 3:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 255, 60),
                        Color.FromArgb(255, 0, 0)
                    };
                case 4:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 255, 247),
                        Color.FromArgb(132, 255, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                case 5:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 170, 255),
                        Color.FromArgb(0, 255, 60),
                        Color.FromArgb(225, 255, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                case 6:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 111, 255),
                        Color.FromArgb(0, 255, 170),
                        Color.FromArgb(55, 255, 0),
                        Color.FromArgb(255, 225, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                case 7:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 72, 255),
                        Color.FromArgb(0, 255, 247),
                        Color.FromArgb(0, 255, 60),
                        Color.FromArgb(132, 255, 0),
                        Color.FromArgb(255, 191, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                case 8:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 47, 255),
                        Color.FromArgb(0, 208, 255),
                        Color.FromArgb(0, 255, 140),
                        Color.FromArgb(26, 255, 0),
                        Color.FromArgb(187, 255, 0),
                        Color.FromArgb(255, 162, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                case 9:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 26, 255),
                        Color.FromArgb(0, 170, 255),
                        Color.FromArgb(0, 255, 200),
                        Color.FromArgb(0, 255, 60),
                        Color.FromArgb(85, 255, 0),
                        Color.FromArgb(225, 255, 0),
                        Color.FromArgb(255, 140, 0),
                        Color.FromArgb(255, 0, 0)
                    };
                default:
                    return new Color[]
                    {
                        Color.FromArgb(115, 0, 255),
                        Color.FromArgb(0, 13, 255),
                        Color.FromArgb(0, 136, 255),
                        Color.FromArgb(0, 255, 247),
                        Color.FromArgb(0, 255, 119),
                        Color.FromArgb(4, 255, 0),
                        Color.FromArgb(132, 255, 0),
                        Color.FromArgb(255, 251, 0),
                        Color.FromArgb(255, 128, 0),
                        Color.FromArgb(255, 0, 0)
                    };
            }
        }

        private void RoundValueRanges(List<float[]> range, int precision)
        {
            foreach (var rangeItem in range)
            {
                var tempItem = rangeItem.Select(y => y = (float)Math.Round(y, precision)).ToArray();
                rangeItem[0] = tempItem[0];
                rangeItem[1] = tempItem[1];
            }
        }
        /// <summary>
        /// GetValueColor
        /// </summary>
        /// <param name="resValue"></param>
        /// <returns></returns>
        public Color GetValueColor(float resValue)
        {
            var color = items.First().Color;

            if (items.Last().Max <= resValue)
                color = items.Last().Color;
            else if (items.First().Min >= resValue)
                color = items.First().Color;
            else
            {
                foreach (var item in items)
                {
                    if (item.Min <= resValue & item.Max >= resValue)
                    {
                        color = item.Color;
                        break;
                    }

                }
            }
            return color;
        }
    }
}
