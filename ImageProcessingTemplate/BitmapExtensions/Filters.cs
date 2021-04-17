using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BitmapExtensions
{
    public static class Filters
    {
        private const double
            W11 = 0.440, W12 = 0.070, W13 = 0.000, 
            W21 = 0.270, W22 = 0.080, W23 = 0.005,
            W31 = 0.150, W32 = 0.060, W33 = 0.020,
            W41 = 0.100, W42 = 0.060, W43 = 0.020,
            W51 = 0.060, W52 = 0.045, W53 = 0.030,
            W61 = 0.060, W62 = 0.060, W63 = 0.025;

        /// <summary> Маска фильтра нижних частот </summary>
        private static double[][,] LowFrequencyFilterMasks =>
            new[]
            {
                new[,]
                {
                    {W13, W13, W13, W13, W13},
                    {W13, W12, W12, W12, W13},
                    {W13, W12, W11, W12, W13},
                    {W13, W12, W12, W12, W13},
                    {W13, W13, W13, W13, W13}
                },
                new[,]
                {
                    {W23, W23, W23, W23, W23},
                    {W23, W22, W22, W22, W23},
                    {W23, W22, W21, W22, W23},
                    {W23, W22, W22, W22, W23},
                    {W23, W23, W23, W23, W23}
                },
                new[,]
                {
                    {W33, W33, W33, W33, W33},
                    {W33, W32, W32, W32, W33},
                    {W33, W32, W31, W32, W33},
                    {W33, W32, W32, W32, W33},
                    {W33, W33, W33, W33, W33}
                },
                new[,]
                {
                    {W43, W43, W43, W43, W43},
                    {W43, W42, W42, W42, W43},
                    {W43, W42, W41, W42, W43},
                    {W43, W42, W42, W42, W43},
                    {W43, W43, W43, W43, W43}
                },
                new[,]
                {
                    {W53, W53, W53, W53, W53},
                    {W53, W52, W52, W52, W53},
                    {W53, W52, W51, W52, W53},
                    {W53, W52, W52, W52, W53},
                    {W53, W53, W53, W53, W53}
                },
                new[,]
                {
                    {W63, W63, W63, W63, W63},
                    {W63, W62, W62, W62, W63},
                    {W63, W62, W61, W62, W63},
                    {W63, W62, W62, W62, W63},
                    {W63, W63, W63, W63, W63}
                },
            };

        /// <summary>
        /// Дополняет изображение пикселями, зеркально отражающими крайние
        /// </summary>
        /// <param name="bm">Исходное изображение</param>
        /// <param name="n">Ширина полосы дополнительных пикселей</param>
        /// <returns>Новый объект Bitmap</returns>
        public static Bitmap SizePlus(this Bitmap bm, int n)
        {
            var bmNew = new Bitmap(bm.Width + 2 * n, bm.Height + 2 * n);
            for (var y = 0; y < bm.Height; y++)
            for (var x = 0; x < bm.Width; x++)
                bmNew.SetPixel(x + n, y + n, bm.GetPixel(x, y));
            for (var x = 0; x < n; x++)
            for (var y = 0; y < n; y++)
            {
                bmNew.SetPixel(x, y, bm.GetPixel(n - 1 - x, n - 1 - y));
                bmNew.SetPixel(x, bmNew.Height - 1 - y, bm.GetPixel(n - 1 - x, bm.Height - n + y));
                bmNew.SetPixel(bmNew.Width - 1 - x, y, bm.GetPixel(bm.Width - n + x, n - y));
                bmNew.SetPixel(bmNew.Width - 1 - x, bmNew.Height - 1 - y,
                    bm.GetPixel(bm.Width - n + x, bm.Height - n + y));
            }

            for (var x = n; x < bmNew.Width - n; x++)
            for (var y = 0; y < n; y++)
            {
                bmNew.SetPixel(x, y, bm.GetPixel(x - n, n - 1 - y));
                bmNew.SetPixel(x, bmNew.Height - 1 - y, bm.GetPixel(x - n, bm.Height - n + y));
            }

            for (var y = n; y < bmNew.Height - n; y++)
            for (var x = 0; x < n; x++)
            {
                bmNew.SetPixel(x, y, bm.GetPixel(n - 1 - x, y - n));
                bmNew.SetPixel(bmNew.Width - 1 - x, y, bm.GetPixel(bm.Width - n + x, y - n));
            }

            return bmNew;
        }

        /// <summary>
        /// Уменьшает размер картинки, обрезая крайние n пикселей
        /// </summary>
        /// <param name="bm">Исходное изображение</param>
        /// <param name="n">Количество пикселей</param>
        /// <returns>Новый объект Bitmap</returns>
        private static Bitmap SizeMinus(this Bitmap bm, int n)
        {
            int width = bm.Width - 2 * n, height = bm.Height - 2 * n;
            var bmNew = new Bitmap(width, height);
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                bmNew.SetPixel(x, y, bm.GetPixel(x + n, y + n));
            return bmNew;
        }

        /// <summary>
        /// Использует метод линейной низкочастотной фильтрации аддитивного шума на исходном изображении
        /// </summary>
        /// <param name="bm">Исходное изображение</param>
        /// <param name="maskIndex">Номер маски фильтра нижних частот</param>
        /// <returns>Новый объект Bitmap - отфильтрованное ихображение</returns>
        public static Bitmap LinearLowFrequencyFilter(this Bitmap bm, int maskIndex) => bm.LowFrequencyFilter(maskIndex, false);

        /// <summary>
        /// Использует метод рекурсивной низкочастотной фильтрации аддитивного шума на исходном изображении
        /// </summary>
        /// <param name="bm">Исходное изображение</param>
        /// <param name="maskIndex">Номер маски фильтра нижних частот</param>
        /// <returns>Новый объект Bitmap - отфильтрованное ихображение</returns>
        public static Bitmap RecursiveLowFrequencyFilter(this Bitmap bm, int maskIndex) => bm.LowFrequencyFilter(maskIndex, true);

        private static Bitmap LowFrequencyFilter(this Bitmap bm, int maskIndex, bool recursive)
        {
            const int n = 2;
            int widthPlusN = bm.Width + n, heightPlusN = bm.Height + n;
            var bmToFilter = bm.SizePlus(n);
            // Если мы перебираем все пиксели по очереди, то функция z должна работать,
            // т.к. y - уже обработанные элементы - как раз будут обработаны до того, как мы доберёмся до центрального элемента матрицы,
            // т.е. записав в bmFiltered ссылку на bmToFilter, мы будем обращаться к одному и тому же экземпляру bitmap
            // как при вызове GetPixel(), так и при вызове SetPixel()
            var bmFiltered = recursive ? bmToFilter : new Bitmap(widthPlusN + n, heightPlusN + n);
            var mask = LowFrequencyFilterMasks[maskIndex];
            for (var y = n; y < heightPlusN; y++)
            for (var x = n; x < widthPlusN; x++)
            {
                int red = 0, green = 0, blue = 0;
                for (var k = 0; k <= n * 2; k++)
                for (var l = 0; l <= n * 2; l++)
                {
                    var color = bmToFilter.GetPixel(x + k - n, y + l - n);
                    red += (int) (color.R * mask[k, l]);
                    green += (int) (color.G * mask[k, l]);
                    blue += (int) (color.B * mask[k, l]);
                }

                bmFiltered.SetPixel(x, y, Color.FromArgb(red, green, blue));
            }

            return bmFiltered.SizeMinus(n);
        }


        public static Bitmap SquaredMedianFilter(this Bitmap bm, int size) => bm.MedianFilter(size, true);

        public static Bitmap CrossedMedianFilter(this Bitmap bm, int size) => bm.MedianFilter(size, false);

        private static Bitmap MedianFilter(this Bitmap bm, int size, bool squared)
        {
            var newBm = bm.SizePlus(size);
            int heightPlusSize = bm.Height + size, widthPlusSize = bm.Width + size;
            for (var y = size; y < heightPlusSize; y++)
            for (var x = size; x < widthPlusSize; x++)
                newBm.SetPixel(x, y, newBm.GetColor(x, y, size, squared));
            return newBm.SizeMinus(size);
        }

        private static Color GetColor(this Bitmap bm, int x, int y, int size, bool squared)
        {
            var window = new List<Color>();
            if (squared)
                for (var row = -size; row <= size; row++)
                for (var col = -size; col <= size; col++)
                    window.Add(bm.GetPixel(x + row, y + col));
            else
                for (var i = -size; i <= size; i++)
                {
                    window.Add(bm.GetPixel(x + i, y));
                    window.Add(bm.GetPixel(x, y + i));
                }

            return window.ToArray().FindMedian();
        }

        private static Color FindMedian(this Color[] colors)
        {
            IEnumerable<int> reds = colors.Select(color => (int) color.R),
                greens = colors.Select(color => (int) color.G),
                blues = colors.Select(color => (int) color.B);
            return Color.FromArgb(reds.ToArray().FindMedian(), greens.ToArray().FindMedian(),
                blues.ToArray().FindMedian());
        }

        private static int FindMedian(this int[] coordinates)
        {
            Array.Sort(coordinates);
            return coordinates[coordinates.Length / 2];
        }

        /// <summary>
        /// Производит оценку качества фильтрации изображения
        /// </summary>
        /// <param name="initial">Исходное изображение</param>
        /// <param name="filtered">Отфильтрованное изображение</param>
        /// <returns>Оценка качества фильтрации</returns>
        public static float FilterQualityRating(Bitmap initial, Bitmap filtered)
        {
            int width = initial.Width, height = initial.Height;
            float d = 0;
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var dxy = initial.GetPixel(x, y).GetBrightness() - filtered.GetPixel(x, y).GetBrightness();
                d += dxy * dxy;
            }

            d /= width * height;
            return d;
        }

        private static int[][,] HighFrequencyFilterMasks =>
            new[]
            {
                new[,]
                {
                    { 1, -2,  1},
                    {-2,  5, -2},
                    { 1, -2,  1}
                },
                new[,]
                {
                    { 0, -1,  0},
                    {-1,  5, -1},
                    { 0, -1,  0}
                },
                new[,]
                {
                    {-1, -1, -1},
                    {-1,  9, -1},
                    {-1, -1, -1}
                }
            };

        public static Bitmap HighFrequencyFilter(this Bitmap bm, int maskIndex)
        {
            var newBm = new Bitmap(bm);//bm.SizePlus(1);
            var mask = HighFrequencyFilterMasks[maskIndex];
            for (var y = 1; y < bm.Height - 1; y++)
            for (var x = 1; x < bm.Width - 1; x++)
            {
                int red = 0, green = 0, blue = 0;
                for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    var color = bm.GetPixel(x + i, y + j);
                    var coef = mask[i + 1, j + 1];
                    red += color.R * coef;
                    green += color.G * coef;
                    blue += color.B * coef;
                }

                newBm.SetPixel(x, y, Color.FromArgb(
                    red > 0 ? red < 256 ? red : 255 : 0, 
                    green > 0 ? green < 256 ? green : 255 : 0, 
                    blue > 0 ? blue < 256 ? blue : 255 : 0));
            }

            return newBm;
        }
    }
}
