using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace PostProc
{
    /// <summary>
    /// ItemsScale
    /// </summary>
    public class ItemsScale
    {
        ItemRange[] items;
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
        /// <inheritdoc/>

        public float MinValue
        {
            get
            {
                return items.First().Min;
            }
        }
        /// <summary>
        /// Precision
        /// </summary>
        public decimal Precision { get; set; } = 4;
        /// <summary>
        /// IndexOf
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int IndexOf(Color color)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Color == color)
                    return i;
            }
            return -1;
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
        /// <summary>
        /// GetItemRanges
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ItemRange> GetItemRanges()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }


        public ItemsScale()
        {
            FillRange(1, 0, 7);
        }

        public ItemsScale(float min, float max, int intervals)
        {
            if (intervals > 7)
                throw new Exception("Превышено максимальное кол-во интервалов");
            else if (intervals < 0)
                throw new Exception("кол-во интервалов не может быть отрицательным");


            FillRange(min, max, intervals);
        }
        /// <summary>
        /// FillRange
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="intervals"></param>
        public void FillRange(float min, float max, decimal intervals)
        {
            items = new ItemRange[(int)intervals];

            var values = GenerateValueRanges(min, max, intervals);
            RoundValueRanges(values);
            var colors = CreateColorRange(intervals);

            for (int i = 0; i < colors.Length; i++)
            {
                items[i] = new ItemRange() { Color = colors[colors.Length - 1 - i], Min = values[i][0], Max = values[i][1] };
            }
        }

        private Color[] CreateColorRange(decimal intervals)
        {
            switch (intervals)
            {
                case 2:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0),
                        Color.FromArgb(0,0,255)
                    };
                case 3:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0),
                        Color.FromArgb(0,255,0),
                        Color.FromArgb(0,0,255)
                    };
                case 4:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0), // красный
                        Color.FromArgb(255,255,0),// желтый
                        Color.FromArgb(0,255,0),// зеленый
                        Color.FromArgb(0,0,255)// синий
                    };
                case 5:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0), // красный
                        Color.FromArgb(255,165,0), // оранжевый
                        Color.FromArgb(255,255,0), // желтый
                        Color.FromArgb(0,128,0), // зеленый
                        Color.FromArgb(0,0,255) // синий
                    };
                case 6:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0), // красный
                        Color.FromArgb(255,165,0), // оранжевый
                        Color.FromArgb(255,255,0), // желтый
                        Color.FromArgb(0,128,0), // зеленый
                        Color.FromArgb(0,0,255), // синий
                        Color.FromArgb(128,0,128) // фиолетовый
                    };
                default:
                    return new Color[]
                    {
                        Color.FromArgb(255,0,0), // красный
                        Color.FromArgb(255,165,0), // оранжевый
                        Color.FromArgb(255,255,0), // желтый
                        Color.FromArgb(0,128,0), // зеленый
                        Color.FromArgb(0,255,255), // голубой
                        Color.FromArgb(0,0,255), // синий
                        Color.FromArgb(128,0,128) // фиолетовый
                    };
            }
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

        private static List<float[]> GenerateValueRanges(float min, float max, decimal intervals)
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
