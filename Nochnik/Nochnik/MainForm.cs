using System;
using System.Windows.Forms;

namespace Nochnik
{
    public partial class MainForm : Form
    {
        public WallpaperClock wallpaperClock = new WallpaperClock();

        public MainForm()
        {
            InitializeComponent();
            CreateProgramTray();
            wallpaperClock.Start(3000);
            KeyDetector.Start(this);
        }

        void CreateProgramTray()
        {
            trayIcon.ContextMenu = new ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(TrayIcon_Exit)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Clock Position", new EventHandler(TrayIcon_ClockProperties)));
            trayIcon.Visible = true;
        }

        void TrayIcon_Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void TrayIcon_ClockProperties(object sender, EventArgs e)
        {
            new ClockPropertiesForm(this).Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            wallpaperClock.RestoreInitialWallpaper();
        }
    }
}
