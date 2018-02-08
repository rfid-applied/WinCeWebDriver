using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimpleWebDriver
{
    public class Screenshot
    {
        enum RasterOperation : uint { SRC_COPY = 0x00CC0020 }

        [DllImport("coredll.dll")]
        static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperation rasterOperation);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        // thanks Chris!
        // https://stackoverflow.com/questions/5510304/taking-screenshot-of-a-device-screen-using-c-sharp
        public static string TakeScreenshot(IntPtr hWnd, System.Drawing.Rectangle bounds)
        {
            if (bounds.IsEmpty)
            {
                bounds = Screen.PrimaryScreen.Bounds;
            }
            IntPtr hdc = GetDC(hWnd);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format16bppRgb565);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                IntPtr dstHdc = graphics.GetHdc();
                BitBlt(dstHdc, 0, 0, bounds.Width, bounds.Height, hdc, 0, 0, RasterOperation.SRC_COPY);
                graphics.ReleaseHdc(dstHdc);
            }
            string base64;
            using (var ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                base64 = Convert.ToBase64String(ms.ToArray());
            }
            ReleaseDC(hWnd, hdc);

            return base64;
        }
    }
}

