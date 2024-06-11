using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RoundedWindowsEdges
{
    public partial class MainWindow : Window
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private AppConfig config;
        private int currentCornerSize;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MainWindow(Rect screenBounds, int cornerSize) : this()
        {
            InitializeForScreen(screenBounds, cornerSize);
            LoadConfig();
        }

        private void InitializeForScreen(Rect screenBounds, int cornerSize)
        {
            this.CornerSize = cornerSize;
            this.Left = screenBounds.Left;
            this.Top = screenBounds.Top;
            this.Width = screenBounds.Width;
            this.Height = screenBounds.Height;

            Debug.WriteLine($"Window initialized: Left = {this.Left}, Top = {this.Top}, Width = {this.Width}, Height = {this.Height}");
        }

        public int CornerSize { get; set; }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        private void WndRoundedWindowsEdges_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide(); // Hide instead of cancelling close to ensure cleanup
        }

        private void WndRoundedWindowsEdges_Loaded(object sender, RoutedEventArgs e)
        {
            Point location = this.PointToScreen(new Point(0, 0));
            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        private void WndRoundedWindowsEdges_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        public void ChangeCornerSize(int size)
        {
            Debug.WriteLine("ChangeCornerSize method called");
            imgCornerTL.Width = size;
            imgCornerTL.Height = size;
            imgCornerTR.Width = size;
            imgCornerTR.Height = size;
            imgCornerBR.Width = size;
            imgCornerBR.Height = size;
            imgCornerBL.Width = size;
            imgCornerBL.Height = size;

            Debug.WriteLine($"Changing corner size to {size}");
            Debug.WriteLine($"imgCornerTL: {imgCornerTL.Visibility}, {imgCornerTL.Width}, {imgCornerTL.Height}");
            Debug.WriteLine($"imgCornerTR: {imgCornerTR.Visibility}, {imgCornerTR.Width}, {imgCornerTR.Height}");
            Debug.WriteLine($"imgCornerBR: {imgCornerBR.Visibility}, {imgCornerBR.Width}, {imgCornerBR.Height}");
            Debug.WriteLine($"imgCornerBL: {imgCornerBL.Visibility}, {imgCornerBL.Width}, {imgCornerBL.Height}");

            // Save the new size to the config file
            config.CornerSize = size;
            config.SaveConfig();

            currentCornerSize = size;
        }

        private void LoadConfig()
        {
            Debug.WriteLine("LoadConfig method called");
            config = AppConfig.LoadConfig();
            ChangeCornerSize(config.CornerSize);
        }

        public int GetCornerSize()
        {
            return currentCornerSize;
        }
    }
}
