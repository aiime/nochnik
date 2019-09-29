using System;
using System.Windows.Forms;

namespace Nochnik
{
    partial class UsersForm : Form
    {
        UserController userController;

        public UsersForm(UserController userController)
        {
            InitializeComponent();

            this.userController = userController;

            UserListBox.DisplayMember = "Name";
            UserListBox.DataSource = userController.users;

        }

        void UserListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusComboBox.Enabled = false;
            StatusComboBox.Items.Clear();

            StatusComboBox.Items.Add("Working");
            StatusComboBox.Items.Add("Resting");
            StatusComboBox.Items.Add("AtHome");
            StatusComboBox.Items.Add("OnHoliday");

            StatusComboBox.Enabled = true;
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (StatusComboBox.SelectedItem)
            {
                case ("Working"):
                    (UserListBox.SelectedItem as User).CurrentStatus = UserStatus.Working;
                    (UserListBox.SelectedItem as User).UpdateUserStatus();
                    break;
                case ("Resting"):
                    (UserListBox.SelectedItem as User).CurrentStatus = UserStatus.Resting;
                    (UserListBox.SelectedItem as User).UpdateUserStatus();
                    break;
                case ("AtHome"):
                    (UserListBox.SelectedItem as User).CurrentStatus = UserStatus.AtHome;
                    (UserListBox.SelectedItem as User).UpdateUserStatus();
                    break;
                case ("OnHoliday"):
                    (UserListBox.SelectedItem as User).CurrentStatus = UserStatus.OnHoliday;
                    (UserListBox.SelectedItem as User).UpdateUserStatus();
                    break;
            }
        }
    }
}
