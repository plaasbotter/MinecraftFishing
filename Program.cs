using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftFishing
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0008;
        private const int MOUSEEVENTF_LEFTUP = 0x0010;
        public static bool firstScan = true;
        public static List<Point> globalFoodStamps = null;
        static void Main(string[] args)
        {
            Task.Run(() => GameLoop(0,0));

            Console.WriteLine("Press any key to close...");
            Console.Read();
        }

        private static void GameLoop(int attempts, int caught)
        {
            Process mainProp = null;
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName.Contains("Java") || process.ProcessName.Contains("java"))
                {
                    if (process.MainWindowTitle.Contains("Minecraft"))
                    {
                        mainProp = process;
                    }
                }
            }
            if (mainProp != null)
            {
                Imaging.BringToFront(mainProp.MainWindowHandle);
                if (firstScan)
                {
                    Thread.Sleep(5000);
                    globalFoodStamps = new List<Point>();
                    using (Bitmap screenshot = Imaging.CaptureApplication(mainProp.MainWindowHandle))
                    {
                        var ListOfPossibleFoodStamps = Imaging.LocateFoodStamps(screenshot);
                        int maxY = int.MinValue;
                        int currentX = 0;
                        int currentXAdd1 = 0;
                        foreach(var point in ListOfPossibleFoodStamps)
                        {
                            if (point.Y >  maxY)
                            {
                                maxY = point.Y;
                            }
                            if (point.X +1 != currentXAdd1)
                            {
                                if (point.X > currentXAdd1)
                                {
                                    currentX = point.X;
                                    currentXAdd1 = currentX + 1;
                                    globalFoodStamps.Add(new Point(currentXAdd1, maxY));
                                }
                            }
                        }
                    }
                    firstScan = false;
                }
                else if (!firstScan)
                {
                    using (Bitmap screenshot = Imaging.CaptureApplication(mainProp.MainWindowHandle))
                    {
                        var newNikes = Imaging.InvestigateFoodStamps(screenshot,globalFoodStamps[globalFoodStamps.Count-1]);
                        Console.WriteLine("*Reading*");
                        if (newNikes.R == 45 && newNikes.G == 35 && newNikes.B == 29)
                        {
                            Console.WriteLine("Fish");
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            Thread.Sleep(100);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            Thread.Sleep(2500);
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            Thread.Sleep(100);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            caught++;
                            Console.WriteLine("STATS:");
                            Console.WriteLine("Caught: " + caught);
                            Console.WriteLine("Attempts: " + (attempts/2));
                        }
                        attempts++; 
                    }
                }
            }
            Thread.Sleep(480);
            GameLoop(attempts,caught);
        }
    }
}
