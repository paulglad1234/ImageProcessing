using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BitmapExtensions
{
    public static class Differentiators
    {
        private static int[,] Gx =>
            new[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };

        private static int[,] Gy =>
            new[,]
            {
                {-1, -2, -1},
                {0, 0, 0},
                {1, 2, 1}
            };

        private static int[,] Laplaсian1 =>
            new[,]
            {
                {0, 1, 0},
                {1, -4, 1},
                {0, 1, 0}
            };

        private static int[,] Laplaсian2 =>
            new[,]
            {
                {1, 0, 1},
                {0, -4, 0},
                {1, 0, 1}
            };

        private static int[,] Laplaсian3 =>
            new[,]
            {
                {1, 4, 1},
                {4, -20, 4},
                {1, 4, 1}
            };

        private static Color ApplyMask(this Bitmap bm, int x, int y, int[,] mask)
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

            return Color.FromArgb(
                red > 0 ? red < 256 ? red : 255 : 0,
                green > 0 ? green < 256 ? green : 255 : 0,
                blue > 0 ? blue < 256 ? blue : 255 : 0);
        }

        private static Bitmap ApplyMask(this Bitmap bm, int[,] mask)
        {
            var newBm = new Bitmap(bm);
            for (var y = 1; y < bm.Height - 1; y++)
            for (var x = 1; x < bm.Width - 1; x++)
                newBm.SetPixel(x, y, bm.ApplyMask(x, y, mask));

            return newBm;
        }

        public static Bitmap VerticalGradient(this Bitmap bm) => bm.ApplyMask(Gy);

        public static Bitmap HorizontalGradient(this Bitmap bm) => bm.ApplyMask(Gx);

        private static int Pythagoras(int a, int b, bool check = true)
        {
            var num = (int) Math.Sqrt(a * a + b * b);
            return check ? num > 0 ? num < 256 ? num : 255 : 0 : num;
        }

        public static Bitmap ModuleGradient(this Bitmap bm)
        {
            var newBm = new Bitmap(bm);
            for (var y = 1; y < bm.Height - 1; y++)
            for (var x = 1; x < bm.Width - 1; x++)
            {
                Color colorH = bm.ApplyMask(x, y, Gx), colorV = bm.ApplyMask(x, y, Gy);
                newBm.SetPixel(x, y, Color.FromArgb(
                    Pythagoras(colorH.R, colorV.R),
                    Pythagoras(colorH.G, colorV.G),
                    Pythagoras(colorH.B, colorV.B)));
            }

            return newBm;
        }

        public static Bitmap Laplacian1(this Bitmap bm) => bm.ApplyMask(Laplaсian1);

        public static Bitmap Laplacian2(this Bitmap bm) => bm.ApplyMask(Laplaсian2);

        public static Bitmap Laplacian3(this Bitmap bm) => bm.ApplyMask(Laplaсian3);

        public static Bitmap Hough(this Bitmap bm)
        {
            var points = new List<Point>();
            for (var y = 1; y < bm.Height - 1; y++)
            for (var x = 1; x < bm.Width - 1; x++)
            {
                Color colorH = bm.ApplyMask(x, y, Gx),
                    colorV = bm.ApplyMask(x, y, Gy),
                    color = Color.FromArgb(
                        Pythagoras(colorH.R, colorV.R),
                        Pythagoras(colorH.G, colorV.G),
                        Pythagoras(colorH.B, colorV.B));

                if (color.R + color.G + color.B > 220)
                    points.Add(new Point(x, y));
            }

            var len = Pythagoras(bm.Height, bm.Width, false);

            var matrix = new int[180, 2 * len]; // Матрица собирающих элементов
            Array.Clear(matrix, 0, 2 * len);

            var max = 0;

            for (var i = 0; i < 180; i++)
            {
                double sinI = Math.Sin((i - 90) * Math.PI / 180), cosI = Math.Cos((i - 90) * Math.PI / 180);

                foreach (var d in points.Select(point => point.X * cosI + point.Y * sinI))
                {
                    var j = (int) d + len;
                    matrix[i, j]++;
                    if (matrix[i, j] > max)
                        max = matrix[i, j];
                }
            }

            var newBm = new Bitmap(180, 2 * len);
            for (var i = 0; i < 180; i++)
            for (var j = 0; j < 2 * len; j++)
            {
                if (matrix[i, j] > max - 80)
                    newBm.SetPixel(i, j, Color.Yellow);
                else if (matrix[i, j] > max - 115)
                    newBm.SetPixel(i, j, Color.DeepPink);
                else if (matrix[i, j] > max - 125)
                    newBm.SetPixel(i, j, Color.Cyan);
                else
                    newBm.SetPixel(i, j, Color.Black);
            }

            return newBm;
        }
    }
}
