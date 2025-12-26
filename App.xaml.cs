using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
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

                // Get the DPI scale for this specific screen
                var dpiScale = GetDpiScaleForScreen(screen);
                Debug.WriteLine($"Screen {i}: Bounds = {bounds}, DPI Scale = {dpiScale}");

                // Convert physical pixels to WPF device-independent units
                var rect = new Rect(
                    bounds.X / dpiScale,
                    bounds.Y / dpiScale,
                    bounds.Width / dpiScale,
                    bounds.Height / dpiScale);

                Debug.WriteLine($"Screen {i}: WPF Rect = {rect}");

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

        private double GetDpiScaleForScreen(Forms.Screen screen)
        {
            try
            {
                // Try to get per-monitor DPI (Windows 8.1+)
                var point = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
                IntPtr hMonitor = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
                
                if (hMonitor != IntPtr.Zero)
                {
                    int result = GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
                    if (result == 0) // S_OK
                    {
                        Debug.WriteLine($"Monitor DPI: {dpiX} x {dpiY}");
                        return dpiX / 96.0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get per-monitor DPI: {ex.Message}");
            }

            // Fallback to system DPI
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g.GetHdc();
                int dpiX = GetDeviceCaps(desktop, 88);
                g.ReleaseHdc(desktop);
                return dpiX / 96.0;
            }
        }

        private const int MDT_EFFECTIVE_DPI = 0;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("Shcore.dll")]
        static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, int dwFlags);
    }
}
