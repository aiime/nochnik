using System;
using System.Windows.Forms;

namespace Nochnik
{
    partial class MainForm : Form
    {
        public WallpaperClock wallpaperClock;
        WallpaperPainter wallpaperPainter;
        UserController userController;

        public MainForm()
        {
            wallpaperPainter = new WallpaperPainter();
            wallpaperClock = new WallpaperClock(wallpaperPainter);
            userController = new UserController(wallpaperPainter);

            InitializeComponent();
            CreateProgramTray();
            wallpaperClock.Start(1000);
            KeyDetector.Start(this);
        }

        void CreateProgramTray()
        {
            trayIcon.ContextMenu = new ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Users", (s, a) => { new UsersForm(userController).Show(); }));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Position", new EventHandler(TrayIcon_ClockProperties)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Color", new EventHandler(TrayIcon_Color)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("-"));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(TrayIcon_Exit)));
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

        void TrayIcon_Color(object sender, EventArgs e)
        {
            new ColorForm(wallpaperClock).Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            wallpaperPainter.SetInitialWallpaper();
        }
    }
}
