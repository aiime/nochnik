using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Nochnik
{
    class UserController : IWallpaperPainterSubscriber
    {
        List<WallpaperPart> userBarParts = new List<WallpaperPart>();
        public List<User> users = new List<User>();
        WallpaperPainter wallpaperPainter;

        public UserController(WallpaperPainter wallpaperPainter)
        {
            this.wallpaperPainter = wallpaperPainter;
            this.wallpaperPainter.Subscribe(this);

            DirectoryInfo[] userDirectories = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Users").GetDirectories();
            
            for (int i = 0; i < userDirectories.Length; i++)
            {
                

                Dictionary<UserStatus, Image> userAvatarByStatus = new Dictionary<UserStatus, Image>();
                FileInfo[] userAvatars = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Users" + @"\" + userDirectories[i].Name).GetFiles();
                userAvatarByStatus.Add(UserStatus.Working, Image.FromFile(userAvatars[0].FullName));
                userAvatarByStatus.Add(UserStatus.Resting, Image.FromFile(userAvatars[1].FullName));
                userAvatarByStatus.Add(UserStatus.AtHome, Image.FromFile(userAvatars[2].FullName));
                userAvatarByStatus.Add(UserStatus.OnHoliday, Image.FromFile(userAvatars[3].FullName));

                User user = new User(userDirectories[i].Name, userAvatarByStatus, UserStatus.Working, this);
                users.Add(user);
            }

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;
            int userCenterY = screenCenterY + twoDigitHeight / 2;
            for (int i = 0; i < users.Count; i++)
            {
                int userCenterX = screenCenterX + colonWidth / 2 + twoDigitWidth - 80 * (i + 1);
                userBarParts.Add(new WallpaperPart(users[i].GetUserAvatar(UserStatus.Working), userCenterX, userCenterY, 80, 105));
            }

            lock (wallpaperPainter)
            {
                wallpaperPainter.SetResultWallpaper();
            }
        }

        public int GetUserCount()
        {
            return users.Count;
        }

        public User GetUser(int n)
        {
            return users[n];
        }

        public void UpdateUserStatuses()
        {
            userBarParts.Clear();

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;
            int userCenterY = screenCenterY + twoDigitHeight / 2;
            for (int i = 0; i < users.Count; i++)
            {
                int userCenterX = screenCenterX + colonWidth / 2 + twoDigitWidth - 80 * (i + 1);
                userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userCenterX, userCenterY, 80, 105));
            }

            lock (wallpaperPainter)
            {
                wallpaperPainter.SetResultWallpaper();
            }
        }

        public List<WallpaperPart> GetWallpaperParts()
        {
            return userBarParts;
        }
    }
}
