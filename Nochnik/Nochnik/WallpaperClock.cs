using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Nochnik
{
    public class WallpaperClock
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);
        static readonly UInt32 SPI_SETDESKWALLPAPER = 20;
        static readonly UInt32 SPIF_UPDATEINIFILE = 0x1;
        static readonly UInt32 SPIF_SENDCHANGE = 0x2;

        static readonly string RESULT_CLOCK_IMAGE_PATH = AppDomain.CurrentDomain.BaseDirectory + @"\clock";
        static readonly Image[] MINUTE_DIGITS = new Image[60];
        static readonly Image[] HOUR_DIGITS = new Image[25];
        static readonly Bitmap RESULT_CLOCK_IMAGE = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        static int hours = 0;
        static int minutes = 0;
        static int seconds = 0;
        static Image hoursImage;
        static Image minutesImage;
        static Image colon = Properties.Resources.colon as Image;

        static Graphics g = Graphics.FromImage(RESULT_CLOCK_IMAGE);
        static Image initialWallpaper;

        public WallpaperClock()
        {
            FillDigitArrays();
            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\CachedFiles");
            string wallpaperPath = directoryInfo.GetFiles()[0].FullName; // TODO: DirectoryNotFoundException handler.
            initialWallpaper = Image.FromFile(wallpaperPath);
            initialWallpaper.Save(AppDomain.CurrentDomain.BaseDirectory + @"ini_wallpaper");
            initialWallpaper = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"ini_wallpaper");

            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;

            hoursX = screenCenterX - twoDigitWidth - colonWidth;
            hoursY = screenCenterY - (twoDigitHeight / 2);

            colonX = screenCenterX - (colonWidth / 2);
            colonY = screenCenterY - (colonHeight / 2) + 15;

            minutesX = screenCenterX + (colonWidth / 2);
            minutesY = screenCenterY - twoDigitHeight / 2;
        }

        public void Start(double interval)
        {
            hours = DateTime.Now.Hour;
            minutes = DateTime.Now.Minute;
            seconds = DateTime.Now.Second;
            UpdateWallpaperClock(hours, minutes);

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerTick;
            timer.Enabled = true;
        }

        public void ChangeClockPosition(int x, int y)
        {
            int screenCenterX = x;
            int screenCenterY = y;

            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;

            hoursX = screenCenterX - twoDigitWidth - colonWidth;
            hoursY = screenCenterY - (twoDigitHeight / 2);

            colonX = screenCenterX - (colonWidth / 2);
            colonY = screenCenterY - (colonHeight / 2) + 15;

            minutesX = screenCenterX + (colonWidth / 2);
            minutesY = screenCenterY - twoDigitHeight / 2;

            UpdateWallpaperClock(hours, minutes);
        }

        public void RestoreInitialWallpaper()
        {
            g.DrawImage(initialWallpaper, 0, 0);
            RESULT_CLOCK_IMAGE.Save(RESULT_CLOCK_IMAGE_PATH, ImageFormat.Jpeg);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, RESULT_CLOCK_IMAGE_PATH, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        void OnTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            seconds++;
            if (seconds < 60) return;
            else seconds = 0;

            if (DateTime.Now.Second < 50) seconds = DateTime.Now.Second; // Time correction.
            minutes++;
            if (minutes == 60)
            {
                minutes = 0;
                hours++;
                if (hours == 25)
                {
                    hours = 0;
                }
            }

            UpdateWallpaperClock(hours, minutes);
        }

        int hoursX;
        int hoursY;
        int colonX;
        int colonY;
        int minutesX;
        int minutesY;

        void UpdateWallpaperClock(int hours, int minutes)
        {
            hoursImage = HOUR_DIGITS[hours];
            minutesImage = MINUTE_DIGITS[minutes];
            
            g.DrawImage(initialWallpaper, 0, 0);
            g.DrawImage(hoursImage, hoursX, hoursY, 600, 430);          
            g.DrawImage(colon, colonX, colonY, 70, 279);
            g.DrawImage(minutesImage, minutesX, minutesY, 600, 430);

            RESULT_CLOCK_IMAGE.Save(RESULT_CLOCK_IMAGE_PATH, ImageFormat.Jpeg);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, RESULT_CLOCK_IMAGE_PATH, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        static void FillDigitArrays()
        {
            for (int i = 0; i < 10; i++)
            {
                MINUTE_DIGITS[i] = Properties.Resources.ResourceManager.GetObject("_0" + i) as Image;
            }
            for (int i = 10; i < 60; i++)
            {
                MINUTE_DIGITS[i] = Properties.Resources.ResourceManager.GetObject("_" + i) as Image;
            }

            for (int i = 0; i < 25; i++)
            {
                HOUR_DIGITS[i] = Properties.Resources.ResourceManager.GetObject("_" + i) as Image;
            }
        }
    }
}
