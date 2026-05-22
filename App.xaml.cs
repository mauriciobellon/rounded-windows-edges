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

        // ── Win32 helpers for per-monitor DPI ────────────────────────────────

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X, Y; }

        // Returns the monitor handle nearest to a given point.
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        // Returns the effective DPI for a specific monitor (Windows 8.1+).
        [DllImport("Shcore.dll", SetLastError = true)]
        private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);
        private const int MDT_EFFECTIVE_DPI = 0;

        // Fallback: reads DPI from a GDI device context (always primary monitor).
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private const int LOGPIXELSX = 88;

        // ── Startup / shutdown ────────────────────────────────────────────────

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

                // BUG FIX 1 & 2:
                // GetDpiScaleForScreen queries *this monitor's* DPI via
                // GetDpiForMonitor instead of always reading the primary
                // monitor's DC. The resulting scale is then applied to the
                // bounds so the Rect is expressed in WPF logical units
                // (96-DPI baseline) rather than raw physical pixels.
                // Previously dpiScale was computed but never used, which meant
                // every window was positioned/sized in physical pixels inside
                // WPF's logical coordinate space — correct only when DPI = 100 %.
                double dpiScale = GetDpiScaleForScreen(screen);
                Debug.WriteLine($"Screen {i}: Bounds={bounds}, DPI Scale={dpiScale}");

                var rect = new Rect(
                    bounds.X      / dpiScale,
                    bounds.Y      / dpiScale,
                    bounds.Width  / dpiScale,
                    bounds.Height / dpiScale
                );

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
                window.Close();

            trayIcon.Dispose();
            base.OnExit(e);
        }

        // ── DPI helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Returns the effective DPI scale for the given screen (1.0 = 100 %,
        /// 1.25 = 125 %, 1.5 = 150 %, etc.).
        /// Uses GetDpiForMonitor (Windows 8.1+) so every monitor in a
        /// multi-monitor setup reports its own DPI independently.
        /// Falls back to the GDI primary-monitor DPI on older Windows versions.
        /// </summary>
        private static double GetDpiScaleForScreen(Forms.Screen screen)
        {
            // Sample the centre of this screen to identify its monitor handle.
            var pt = new POINT
            {
                X = screen.Bounds.Left + screen.Bounds.Width  / 2,
                Y = screen.Bounds.Top  + screen.Bounds.Height / 2
            };

            IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);

            // GetDpiForMonitor returns 0 (S_OK) on success.
            if (GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, out uint dpiX, out uint _) == 0)
            {
                Debug.WriteLine($"  GetDpiForMonitor -> {dpiX} DPI");
                return dpiX / 96.0;
            }

            // Fallback: GDI gives the primary monitor's DPI; acceptable when
            // all monitors share the same DPI setting.
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hdc = g.GetHdc();
                int dpi = GetDeviceCaps(hdc, LOGPIXELSX);
                g.ReleaseHdc(hdc);
                Debug.WriteLine($"  GetDeviceCaps fallback -> {dpi} DPI");
                return dpi / 96.0;
            }
        }
    }
}