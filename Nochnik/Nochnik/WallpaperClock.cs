using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Nochnik
{
    class WallpaperClock : IWallpaperPainterSubscriber
    {
        readonly Image[] MINUTE_DIGITS = new Image[60];
        readonly Image[] HOUR_DIGITS = new Image[25];
        readonly Image COLON = Properties.Resources.colon as Image;
        Image coloredHours;
        Image coloredColon;
        Image coloredMinutes;

        int hours = 0;
        int minutes = 0;
        int seconds = 0;

        int hoursX;
        int hoursY;
        int colonX;
        int colonY;
        int minutesX;
        int minutesY;

        int r = 0;
        int g = 0;
        int b = 0;

        WallpaperPainter wallpaperPainter;
        List<WallpaperPart> clockParts = new List<WallpaperPart>();

        public WallpaperClock(WallpaperPainter wallpaperPainter)
        {
            this.wallpaperPainter = wallpaperPainter;
            this.wallpaperPainter.Subscribe(this);
            FillDigitArrays();

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

        System.Timers.Timer timer;
        public void Start(double interval)
        {
            timerStopped = false;
            hours = DateTime.Now.Hour;
            minutes = DateTime.Now.Minute;
            seconds = DateTime.Now.Second;
            UpdateWallpaperClock(hours, minutes);

            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnTimerTick;
            timer.Enabled = true;
        }

        bool timerStopped;
        public void Stop()
        {
            timer.Enabled = false;
            timer.Elapsed -= OnTimerTick;
            clockParts.Clear();
            lock (wallpaperPainter)
            {
                timerStopped = true;
                wallpaperPainter.SetResultWallpaper();
            }
        }

        public void ChangeClockPosition(int x, int y)
        {
            int screenCenterX = x;
            int screenCenterY = y;

            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._00.Width;
            int twoDigitHeight = Properties.Resources._00.Height;

            hoursX = screenCenterX - twoDigitWidth - colonWidth;
            hoursY = screenCenterY - (twoDigitHeight / 2);

            colonX = screenCenterX - (colonWidth / 2);
            colonY = screenCenterY - (colonHeight / 2) + 15;

            minutesX = screenCenterX + (colonWidth / 2);
            minutesY = screenCenterY - twoDigitHeight / 2;

            UpdateWallpaperClock(hours, minutes);
        }

        public void ChangeClockColor(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;

            UpdateWallpaperClock(hours, minutes);
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
                if (hours == 24)
                {
                    hours = 0;
                }
            }

            UpdateWallpaperClock(hours, minutes);
        }

        void UpdateWallpaperClock(int hours, int minutes)
        {
            coloredHours?.Dispose();
            coloredColon?.Dispose();
            coloredMinutes?.Dispose();

            coloredHours = ColorImage(HOUR_DIGITS[hours], r, g, b);           
            coloredColon = ColorImage(COLON, r, g, b);         
            coloredMinutes = ColorImage(MINUTE_DIGITS[minutes], r, g, b);

            clockParts.Clear();
            clockParts.Add(new WallpaperPart(coloredHours, hoursX, hoursY, 600, 430));
            clockParts.Add(new WallpaperPart(coloredColon, colonX, colonY, 70, 279));
            clockParts.Add(new WallpaperPart(coloredMinutes, minutesX, minutesY, 600, 430));

            lock (wallpaperPainter)
            {
                if (!timerStopped) wallpaperPainter.SetResultWallpaper();             
            }        
        }

        Bitmap ColorImage(Image image, int r, int g, int b)
        {
            Bitmap bitmap = new Bitmap(image);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    byte alpha = bitmap.GetPixel(x, y).A;
                    if (alpha > 0) bitmap.SetPixel(x, y, Color.FromArgb(alpha, r, g, b));
                }
            }

            return bitmap;
        }

        void FillDigitArrays()
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

        public List<WallpaperPart> GetWallpaperParts()
        {
            return clockParts;
        }
    }
}
