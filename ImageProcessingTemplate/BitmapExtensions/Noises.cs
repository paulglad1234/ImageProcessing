using System;
using System.Drawing;

namespace BitmapExtensions
{
    public static class Noises
    {
        private static Random Rnd { get; } = new Random();

        /// <summary>Уровень шума</summary>
        public static int Intensity { get; set; }

        /// <summary>
        /// Добавляет аддитивный шум к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap AdditiveNoise(this Bitmap bitmap)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            var d = Intensity * Rnd.NextDouble();
            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                int red = (int) (color.R + d),
                    green = (int) (color.G + d),
                    blue = (int) (color.B + d);
                red = red > 255 ? 255 : red < 0 ? 0 : red;
                green = green > 255 ? 255 : green < 0 ? 0 : green;
                blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;
                newBitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
            }

            return newBitmap;
        }

        /// <summary>
        /// Добавляет белый шум к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap WhiteNoise(this Bitmap bitmap)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                var d = Intensity * Rnd.NextDouble();
                int red = (int) (color.R + d),
                    green = (int) (color.G + d),
                    blue = (int) (color.B + d);
                red = red > 255 ? 255 : red < 0 ? 0 : red;
                green = green > 255 ? 255 : green < 0 ? 0 : green;
                blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;
                newBitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
            }

            return newBitmap;
        }

        /// <returns>Значение отклонения для гауссовского шума</returns>
        private static int GaussianValue()
        {
            var sum = 0.0;
            for (var i = 0; i < 12; i++)
            {
                sum += Rnd.NextDouble();
            }

            return (int) (sum - 6) * Intensity;
        }

        /// <summary>
        /// Добавляет гауссовский шум к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <param name="grey">true - если серый шум, false - если цветной</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        private static Bitmap GaussianNoise(this Bitmap bitmap, bool grey)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                int red, green, blue;
                if (grey)
                {
                    var d = GaussianValue();
                    red = color.R + d;
                    green = color.G + d;
                    blue = color.B + d;
                }
                else
                {
                    red = color.R + GaussianValue();
                    green = color.G + GaussianValue();
                    blue = color.B + GaussianValue();
                }

                red = red > 255 ? 255 : red < 0 ? 0 : red;
                green = green > 255 ? 255 : green < 0 ? 0 : green;
                blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;
                newBitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
            }

            return newBitmap;
        }

        /// <summary>
        /// Добавляет серый гауссовский шум к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap GreyGaussianNoise(this Bitmap bitmap) => GaussianNoise(bitmap, true);

        /// <summary>
        /// Добавляет цветной гауссовский шум к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap ColoredGaussianNoise(this Bitmap bitmap) => GaussianNoise(bitmap, false);

        /// <summary>
        /// Добавляет импульсную помеху к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <param name="salt">true => salt, false => pepper, null => salt-pepper</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        private static Bitmap ImpulseNoise(this Bitmap bitmap, bool? salt)
        {
            var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
                newBitmap.SetPixel(x, y, bitmap.GetPixel(x, y));

            var intensity =
                bitmap.Width * bitmap.Height * Intensity *
                0.001; //в данном случае интенсивность - доля от общего количества пикселей
            var saltPepper = salt == null;
            var color = !saltPepper && (bool) salt ? Color.White : Color.Black;
            for (var i = 0; i < intensity; i++)
                newBitmap.SetPixel(Rnd.Next(bitmap.Width), Rnd.Next(bitmap.Height),
                    saltPepper ? (Rnd.Next(2) == 0 ? Color.White : Color.Black) : color);

            return newBitmap;
        }

        /// <summary>
        /// Добавляет соль-импульсную помеху к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap SaltNoise(this Bitmap bitmap) => ImpulseNoise(bitmap, true);

        /// <summary>
        /// Добавляет перец-импульсную помеху к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap PepperNoise(this Bitmap bitmap) => ImpulseNoise(bitmap, false);

        /// <summary>
        /// Добавляет соль-перец-импульсную помеху к исходному изображению
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <returns>Новоый объект Bitmap - обработанное изображение</returns>
        public static Bitmap SaltPepperNoise(this Bitmap bitmap) => ImpulseNoise(bitmap, null);
    }
}
