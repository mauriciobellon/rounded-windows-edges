using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using Forms = System.Windows.Forms;

namespace RoundedWindowsEdges
{
    public partial class App : Application
    {
        private TrayIcon trayIcon;
        private MainWindow[] mainWindows;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppConfig config = AppConfig.LoadConfig();
            int screenCount = Forms.Screen.AllScreens.Length;
            mainWindows = new MainWindow[screenCount];

            for (int i = 0; i < screenCount; i++)
            {
                var screen = Forms.Screen.AllScreens[i];
                var bounds = screen.Bounds;

                var dpiScale = GetDpiScale(screen);
                Debug.WriteLine($"Screen {i}: Bounds = {bounds}, DPI Scale = {dpiScale}");

                var rect = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);

                if (mainWindows[i] == null)
                {
                    mainWindows[i] = new MainWindow(rect, config.CornerSize);
                    mainWindows[i].Show();
                }
            }

            trayIcon = new TrayIcon(mainWindows[0]);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var window in mainWindows)
            {
                window.Close();
            }

            trayIcon.Dispose();
            base.OnExit(e);
        }

        private double GetDpiScale(Forms.Screen screen)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g.GetHdc();
                int dpiX = GetDeviceCaps(desktop, 88); 
                int dpiY = GetDeviceCaps(desktop, 90);
                g.ReleaseHdc(desktop);

                return dpiX / 96.0; 
            }
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    }
}
