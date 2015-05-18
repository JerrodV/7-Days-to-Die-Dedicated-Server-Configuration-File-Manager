using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SevenDaysConfigUI.Models;
using System.Net;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace SevenDaysConfigUI.Pages
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Page
    {
        public SteamUser SteamUser;

        private Boolean _userVerified = false;
        private Boolean userVerified
        {
            get 
            {
                return _userVerified;
            }

            set 
            {
                _userVerified = value;
                btnSaveUser.IsEnabled = value;
            }
        }

        public AddUser()
        {
            InitializeComponent();
            SteamUser = new SteamUser();
        }

        private void btnCancelAddUder_Click(object sender, RoutedEventArgs e)
        {
            this.SteamUser = null;
            Window.GetWindow(this).Close();
        }

        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void btnVerifyUser_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(txtSteamID.Text, "^[0-9]+$") && txtSteamID.Text.Length == 17)
            {
                this.SteamUser.SteamID = txtSteamID.Text;
                this.SteamUser.PermissionLevel = -1;
                this.SteamUser.SteamUpdated += SteamUser_SteamUpdated;
                this.SteamUser.GetSteamData();
            }            
        }

        void SteamUser_SteamUpdated(object sender, EventArgs e)
        {            
            grdUserDetails.DataContext = this.SteamUser;
            grdUserDetails.Visibility = System.Windows.Visibility.Visible;
            userVerified = true;
        }

        private void txtSteamID_TextChanged(object sender, TextChangedEventArgs e)
        {
            userVerified = false;
            if (Regex.IsMatch(txtSteamID.Text, "^[0-9]+$") && txtSteamID.Text.Length == 17)
            {
                btnVerifyUser.IsEnabled = true;
            }
            else
            {
                btnVerifyUser.IsEnabled = false;
            }
        }
    }
}
