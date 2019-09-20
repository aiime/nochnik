using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Nochnik
{
    class UserController : IWallpaperPainterSubscriber
    {
        const int MAX_USERS = 3;
        List<WallpaperPart> userBarParts = new List<WallpaperPart>();
        List<Image> users = new List<Image>();

        public UserController(WallpaperPainter wallpaperPainter)
        {
            wallpaperPainter.Subscribe(this);

            FileInfo[] userPictures = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.png");

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;

            for (int i = 0; i < userPictures.Length && i < MAX_USERS; i++)
            {
                int userCenterX = screenCenterX + colonWidth / 2 + twoDigitWidth - 80 * (i + 1);
                int userCenterY = screenCenterY + twoDigitHeight / 2;
                users.Add(Image.FromFile(userPictures[i].FullName));
                userBarParts.Add(new WallpaperPart(Image.FromFile(userPictures[i].FullName), userCenterX, userCenterY, 80, 105));
            }

            lock (wallpaperPainter)
            {
                wallpaperPainter.SetResultWallpaper();
            }
        }

        public void AddNochnik(int n)
        {
             
        }

        public List<WallpaperPart> GetWallpaperParts()
        {
            return userBarParts;
        }
    }
}
