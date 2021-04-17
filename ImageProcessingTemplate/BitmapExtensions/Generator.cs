using System;
using System.Drawing;

namespace BitmapExtensions
{
    public static class Generator
    {
        /// <returns>Треугольник Максвелла</returns>>
        public static Bitmap MaxwellTriangle
        {
            get
            {
                var bm = new Bitmap(256, 256);
                for (var g = 0; g < 256; g++)
                for (var r = 0; r < 256 - g; r++)
                    bm.SetPixel(r, g, Color.FromArgb(r, g, 255 - r - g));
                return bm;
            }
        }

        /// <summary>
        /// Генерирует изображение плоской волны
        /// </summary>
        /// <param name="width">Ширина в пикселях</param>
        /// <param name="height">Высота в пикселях</param>
        /// <param name="u">Горизонтальная пространственная частота</param>
        /// <param name="v">Вертикальная пространственная частота</param>
        /// <returns>Новоый объект Bitmap</returns>
        public static Bitmap FlatWave(int width, int height, double u, double v)
        {
            var newBitmap = new Bitmap(width, height);
            const int a = 50, b = 127;
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var l = (int)(a * Math.Cos(u * x + v * y) + b);
                newBitmap.SetPixel(x, y, Color.FromArgb(l, l, l));
            }

            return newBitmap;
        }
    }
}
