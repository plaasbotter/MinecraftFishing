using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MinecraftFishing
{
    public class Imaging
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public static IntPtr GetProcess(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length > 0)
            {
                return processes[0].MainWindowHandle;
            }
            else
            {
                return new IntPtr();
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static void BringToFront(IntPtr pTemp)
        {
            ShowWindow(pTemp, 9);
        }
        public static Bitmap CaptureApplication(IntPtr hwnd)
        {
            var rect = new RECT();
            GetWindowRect(hwnd, out rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }

            return bmp;
        }
        public static List<Point> LocateFoodStamps(Bitmap img)
        {
            List<Point> points = new List<Point>();

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    var col = img.GetPixel(x, y);
                    if (col.R == 223 && col.G == 177 && col.B == 143)
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }
            return points;
        }
        public static Color InvestigateFoodStamps(Bitmap img,Point point)
        {
            Color col = Color.White;
            col = img.GetPixel(point.X, point.Y);
            return col;
        }
    }
}