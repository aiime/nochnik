using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System;

namespace Nochnik
{
    enum UserStatus
    {
        Working,
        Resting,
        AtHome,
        OnHoliday
    };

    class User
    {
        public string Name { get; private set; }
        public string Number { get; private set; }

        public int ShiftsLeft
        {
            get
            {
                return shiftsLeft;
            }
            set
            {
                shiftsLeft = value;
                XmlNode root = UserInfo.DocumentElement;
                XmlNode shiftsLeftNode = root.SelectSingleNode("/user_data/shifts_left[1]");
                shiftsLeftNode.InnerText = shiftsLeft.ToString();
                UserInfo.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Users\" + Name + @"\user_info.xml");
            }
        }
        int shiftsLeft;

        public int ShiftsCompleted
        {
            get
            {
                return shiftsCompleted;
            }
            set
            {
                Console.WriteLine(value);
                shiftsCompleted = value;
                XmlNode root = UserInfo.DocumentElement;
                XmlNode shiftsCompletedNode = root.SelectSingleNode("/user_data/shifts_completed[1]");
                shiftsCompletedNode.InnerText = shiftsCompleted.ToString();
                UserInfo.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Users\" + Name + @"\user_info.xml");
            }
        }
        int shiftsCompleted;

        public XmlDocument UserInfo { get; private set; }

        public UserStatus currentStatus;
        Dictionary<UserStatus, Image> avatarByStatus;
        UserController userController;
        

        public User(string name,
                    string number,
                    XmlDocument userInfo,
                    Dictionary<UserStatus, Image> avatarByStatus,
                    UserStatus currentStatus,
                    UserController userController,
                    int shiftsLeft)
        {
            Name = name;
            Number = number;
            this.UserInfo = userInfo;
            this.avatarByStatus = avatarByStatus;
            this.currentStatus = currentStatus;
            this.userController = userController;
            this.ShiftsLeft = shiftsLeft;
            this.ShiftsCompleted = Int32.Parse(userInfo.GetElementsByTagName("shifts_completed")[0].InnerText);
        }

        public void UpdateUserStatus()
        {
            userController.UpdateUserStatuses();
        }

        public Image GetUserAvatar(UserStatus userStatus)
        {
            return avatarByStatus[userStatus];
        }

        public Image GetCurrentUserAvatar()
        {
            return avatarByStatus[currentStatus];
        }
    }
}
