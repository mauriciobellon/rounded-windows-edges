using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RoundedWindowsEdges
{
    public class TrayIcon : IDisposable
    {
        private NotifyIcon notifyIcon;
        private MainWindow mainWindow;

        private ToolStripMenuItem smallCornersItem;
        private ToolStripMenuItem mediumCornersItem;
        private ToolStripMenuItem largeCornersItem;
        private ToolStripMenuItem autoStartItem;

        public TrayIcon(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.AppIcon,
                Visible = true,
                Text = "Rounded Screen"
            };

            var contextMenu = new ContextMenuStrip();
            smallCornersItem = new ToolStripMenuItem("Small Corners", null, (sender, e) => ChangeCornerSize(10));
            mediumCornersItem = new ToolStripMenuItem("Medium Corners", null, (sender, e) => ChangeCornerSize(20));
            largeCornersItem = new ToolStripMenuItem("Large Corners", null, (sender, e) => ChangeCornerSize(30));

            autoStartItem = new ToolStripMenuItem("Auto Start", null, ToggleAutoStart)
            {
                Checked = IsAutoStartEnabled()
            };

            contextMenu.Items.Add(smallCornersItem);
            contextMenu.Items.Add(mediumCornersItem);
            contextMenu.Items.Add(largeCornersItem);
            contextMenu.Items.Add(autoStartItem);
            contextMenu.Items.Add("Exit", null, OnExit);

            notifyIcon.ContextMenuStrip = contextMenu;

            UpdateCornerSizeCheck(mainWindow.GetCornerSize());
        }

        private void ChangeCornerSize(int size)
        {
            mainWindow.ChangeCornerSize(size);
            UpdateCornerSizeCheck(size);
        }

        private void UpdateCornerSizeCheck(int size)
        {
            smallCornersItem.Checked = (size == 10);
            mediumCornersItem.Checked = (size == 20);
            largeCornersItem.Checked = (size == 30);
        }

        private void ToggleAutoStart(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                if (menuItem.Checked)
                {
                    DisableAutoStart();
                    menuItem.Checked = false;
                }
                else
                {
                    EnableAutoStart();
                    menuItem.Checked = true;
                }
            }
        }

        private bool IsAutoStartEnabled()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return rk.GetValue("RoundedWindowsEdges") != null;
        }

        private void EnableAutoStart()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("RoundedWindowsEdges", Process.GetCurrentProcess().MainModule.FileName);
        }

        private void DisableAutoStart()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.DeleteValue("RoundedWindowsEdges", false);
        }

        private void OnExit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }
    }
}
