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
        public string Name { get; set; }

        public UserStatus CurrentStatus
        {
            get
            {
                return currentStatus;
            }
            set
            {
                currentStatus = value;
                userController.UpdateUserStatuses();
            }
        }

        UserStatus currentStatus;
        Dictionary<UserStatus, Image> avatarByStatus;
        UserController userController;

        public User(string name, Dictionary<UserStatus, Image> avatarByStatus, UserStatus currentStatus, UserController userController)
        {
            Name = name;
            this.avatarByStatus = avatarByStatus;
            this.currentStatus = UserStatus.Working;
            this.userController = userController;            
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
