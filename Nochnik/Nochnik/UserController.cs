using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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

                XmlDocument userInfo = new XmlDocument();
                userInfo.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Users\" + userDirectories[i].Name + @"\user_info.xml");
                string userName = userInfo.GetElementsByTagName("name")[0].InnerText;
                string userNumber = userInfo.GetElementsByTagName("number")[0].InnerText;
                int shiftsLeft = Int32.Parse(userInfo.GetElementsByTagName("shifts_left")[0].InnerText);

                User user = new User(userName, userNumber, userInfo, userAvatarByStatus, UserStatus.AtHome, this, shiftsLeft);
                users.Add(user);
            }

            SetUserStatusesAccordingToSchedule();
            SortUsersByStatuses();

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;
            int userAvatar_y = screenCenterY + twoDigitHeight / 2;
            bool wasContract = false;
            int contracts = 0;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].UserInfo.GetElementsByTagName("contract")[0].InnerText == "yes")
                {
                    contracts++;
                    wasContract = true;
                    // Добавляем отображение аватара пользователя.
                    int userAvatar_x =
                        (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1)) -
                        ((Properties.Resources.stick.Width + 3 + Properties.Resources.square.Width + 3 + Properties.Resources.circle.Width));
                    userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));

                    // Добавляем отображение оставшихся смен пользователя.
                    int userShiftView_x = userAvatar_x + 80 - 3;
                    int userShiftView_y = userAvatar_y + 105;
                    List<WallpaperPart> userShiftView = ShiftView.UpdateUserShiftView(users[i], userShiftView_x, userShiftView_y);
                    userBarParts.AddRange(userShiftView);
                }
                else
                {
                    if (wasContract)
                    {
                        int userAvatar_x =
                        (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1)) -
                        ((Properties.Resources.stick.Width + 3 + Properties.Resources.square.Width + 3 + Properties.Resources.circle.Width) * contracts);
                        userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));
                    }
                    else
                    {
                        int userAvatar_x = (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1));
                        userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));
                    }
                }
            }         

            lock (wallpaperPainter)
            {
                wallpaperPainter.SetResultWallpaper();
            }
        }

        public void UpdateUserStatuses()
        {
            userBarParts.Clear();
            SortUsersByStatuses();
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int colonWidth = Properties.Resources.colon.Width;
            int colonHeight = Properties.Resources.colon.Height;
            int twoDigitWidth = Properties.Resources._0.Width;
            int twoDigitHeight = Properties.Resources._0.Height;
            int userAvatar_y = screenCenterY + twoDigitHeight / 2;
            bool wasContract = false;
            int contracts = 0;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].UserInfo.GetElementsByTagName("contract")[0].InnerText == "yes")
                {
                    contracts++;
                    wasContract = true;
                    // Добавляем отображение аватара пользователя.
                    int userAvatar_x =
                        (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1)) -
                        ((Properties.Resources.stick.Width + 3 + Properties.Resources.square.Width + 3 + Properties.Resources.circle.Width));
                    userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));

                    // Добавляем отображение оставшихся смен пользователя.
                    int userShiftView_x = userAvatar_x + 80 - 3;
                    int userShiftView_y = userAvatar_y + 105;
                    List<WallpaperPart> userShiftView = ShiftView.UpdateUserShiftView(users[i], userShiftView_x, userShiftView_y);
                    userBarParts.AddRange(userShiftView);
                }
                else
                {
                    if (wasContract)
                    {
                        int userAvatar_x =
                        (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1)) -
                        ((Properties.Resources.stick.Width + 3 + Properties.Resources.square.Width + 3 + Properties.Resources.circle.Width) * contracts);
                        userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));
                    }
                    else
                    {
                        int userAvatar_x = (screenCenterX + colonWidth / 2 + twoDigitWidth) - (80 * (i + 1));
                        userBarParts.Add(new WallpaperPart(users[i].GetCurrentUserAvatar(), userAvatar_x, userAvatar_y, 80, 105));
                    }
                }
            }

            lock (wallpaperPainter)
            {
                wallpaperPainter.SetResultWallpaper();
            }
        }

        List<WallpaperPart> IWallpaperPainterSubscriber.GetWallpaperParts()
        {
            return userBarParts;
        }

        public void SetUserStatusesAccordingToSchedule()
        {
            DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek;
            XmlDocument schedule = new XmlDocument();
            schedule.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Users\schedule.xml");
            string userNumbersWithSeparators = schedule.GetElementsByTagName(currentDayOfWeek.ToString())[0].InnerText;
            string[] userNumbers = userNumbersWithSeparators.Split('-');

            for (int i = 0; i < userNumbers.Length; i++)
            {
                users.Find(user => user.Number == userNumbers[i]).currentStatus = UserStatus.Working;
            }
        }

        void SortUsersByStatuses()
        {
            users.Sort((x, y) =>
            {
                if (x.currentStatus == UserStatus.AtHome && y.currentStatus != UserStatus.AtHome) return 1;
                else if (x.currentStatus != UserStatus.AtHome && y.currentStatus == UserStatus.AtHome) return -1;
                else return 0;
            });
        }
    }
}
