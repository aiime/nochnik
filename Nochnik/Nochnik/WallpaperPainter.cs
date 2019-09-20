using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Nochnik
{
    class WallpaperPainter
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);
        const UInt32 SPI_SETDESKWALLPAPER = 20;
        const UInt32 SPIF_UPDATEINIFILE = 0x1;
        const UInt32 SPIF_SENDCHANGE = 0x2;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SetThreadExecutionState([In] uint esFlags);
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;        

        readonly string WALLPAPER_CACHE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\CachedFiles";
        readonly string RESULT_WALLPAPER_PATH = AppDomain.CurrentDomain.BaseDirectory + @"\result_wallpaper";

        List<IWallpaperPainterSubscriber> subscribers = new List<IWallpaperPainterSubscriber>();
        Image initialWallpaper;
        Bitmap resultWallpaper;
        Graphics painter;

        public WallpaperPainter()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
            AppDomain.CurrentDomain.UnhandledException += (obj, args) => SetThreadExecutionState(ES_CONTINUOUS);

            SaveInitialWallpaper();          
            resultWallpaper = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            painter = Graphics.FromImage(resultWallpaper);
            ResetPainter();

        }
        ~WallpaperPainter()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        public void Subscribe(IWallpaperPainterSubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void SetResultWallpaper()
        {
            if (CachedWallpaperExists())
            {
                ClearWallpaperCache();
            }

            DrawResultWallpaper();

            resultWallpaper.Save(RESULT_WALLPAPER_PATH, ImageFormat.Jpeg);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, RESULT_WALLPAPER_PATH, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            ResetPainter();
        }

        public void SetInitialWallpaper()
        {
            painter.DrawImage(initialWallpaper, 0, 0);
            resultWallpaper.Save(RESULT_WALLPAPER_PATH, ImageFormat.Jpeg);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, RESULT_WALLPAPER_PATH, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
        void DrawPart(Image part, int x, int y, int width, int height)
        {
            painter.DrawImage(part, x, y, width, height);
        }

        void DrawResultWallpaper()
        {
            foreach (IWallpaperPainterSubscriber subscriber in subscribers)
            {
                foreach (WallpaperPart part in subscriber.GetWallpaperParts())
                {
                    DrawPart(part.image, part.x, part.y, part.width, part.height);
                }
            }
        }

        void SaveInitialWallpaper()
        {
            string wallpaperPath = "";
            DirectoryInfo wallpaperCache = new DirectoryInfo(WALLPAPER_CACHE_PATH);

            try
            {
                wallpaperPath = wallpaperCache.GetFiles()[0].FullName;
            }
            catch (DirectoryNotFoundException)
            {
                wallpaperPath = Array.Find(wallpaperCache.Parent.GetFiles(), fileInfo => fileInfo.Name == "TranscodedWallpaper").FullName;
            }

            initialWallpaper = Image.FromFile(wallpaperPath);
            initialWallpaper.Save(AppDomain.CurrentDomain.BaseDirectory + @"ini_wallpaper");
            initialWallpaper = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"ini_wallpaper");
        }

        bool CachedWallpaperExists()
        {
            DirectoryInfo wallpaperCache = new DirectoryInfo(WALLPAPER_CACHE_PATH);
            return (wallpaperCache.Exists && wallpaperCache.GetFiles().Length > 0);
        }

        void ClearWallpaperCache()
        {
            Directory.Delete(WALLPAPER_CACHE_PATH, true);
        }

        void ResetPainter()
        {
            painter.DrawImage(initialWallpaper, 0, 0);
        }
    }
}
