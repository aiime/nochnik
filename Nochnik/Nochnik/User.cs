using System.Drawing;
using System.Collections.Generic;

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

        public UserStatus CurrentStatus
        {
            get
            {
                return currentStatus;
            }
            set
            {
                currentStatus = value;
            }
        }

        UserStatus currentStatus;
        Dictionary<UserStatus, Image> avatarByStatus;
        UserController userController;

        public User(string name, string number, Dictionary<UserStatus, Image> avatarByStatus, UserStatus currentStatus, UserController userController)
        {
            Name = name;
            Number = number;
            this.avatarByStatus = avatarByStatus;
            this.currentStatus = currentStatus;
            this.userController = userController;            
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
