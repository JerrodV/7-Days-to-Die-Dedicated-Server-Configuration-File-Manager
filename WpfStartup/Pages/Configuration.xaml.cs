using Microsoft.Win32;
using SevenDaysConfigUI.Helpers.Validation;
using SevenDaysConfigUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf;
using b = SevenDaysConfigUI.Helpers.Bindings;

namespace SevenDaysConfigUI.Pages
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Page
    {
        #region Local Vars

        List<Exception> errors = new List<Exception>();

        #region Configuration
        String ConfigPath = "";
        ServerConfig configuration;
        BackgroundWorker LoadConfigBW;
        BackgroundWorker SaveConfigBW;        

        List<KeyValuePair<Int32, String>> GameModes;
        List<KeyValuePair<Int32, String>> DropOnDeath;
        List<KeyValuePair<Int32, String>> DropOnQuit;
        List<KeyValuePair<Int32, String>> EnemeySpawnMode;
        List<KeyValuePair<Int32, String>> CraftTimer;
        List<KeyValuePair<Int32, String>> LootTimer;
        List<KeyValuePair<Int32, String>> LandClaimDecayMode;
        List<KeyValuePair<Int32, String>> GameWorld;

        Boolean _configPathExists;
        Boolean configPathExits
        {
            get
            {
                return _configPathExists;
            }

            set
            {
                _configPathExists = value;
                btnLoadConfig.IsEnabled = value;                
                btnClearConfigPath.IsEnabled = value;
                if (!configPathExits && !adminPathExits)
                {
                    btnLoadAll.IsEnabled = false;
                }
                else
                {
                    btnLoadAll.IsEnabled = true;
                }
            }
        }

        Boolean _configLoaded;
        Boolean configLoaded
        {
            get
            {
                return _configLoaded;
            }

            set 
            {
                _configLoaded = value;
                btnSaveConfig.IsEnabled = value;
                btnBackupConfig.IsEnabled = value;                
                if (!configLoaded && !adminLoaded)
                {
                    btnSaveAll.IsEnabled = false;
                }
                else
                {
                    btnSaveAll.IsEnabled = true;
                }
            }
        }
        #endregion

        #region Admin
        String AdminPath = "";
        Admin admin;

        BackgroundWorker LoadAdminBW;
        BackgroundWorker SaveAdminBW;

        Boolean _adminPathExists;
        Boolean adminPathExits
        {
            get
            {
                return _adminPathExists;
            }

            set
            {
                _adminPathExists = value;
                btnLoadAdmin.IsEnabled = value;                
                btnLoadAdmin.IsEnabled = value;
                btnClearAdminPath.IsEnabled = value;
                if (!configPathExits && !adminPathExits)
                {
                    btnLoadAll.IsEnabled = false;
                }
                else
                {
                    btnLoadAll.IsEnabled = true;
                }
            }
        }

        Boolean _adminLoaded;
        Boolean adminLoaded
        {
            get
            {
                return _adminLoaded;
            }

            set
            {
                _adminLoaded = value;
                btnBackupAdmin.IsEnabled = value;
                btnSaveAdmin.IsEnabled = value;
                if (!configLoaded && !adminLoaded)
                {
                    btnSaveAll.IsEnabled = false;
                }
                else
                {
                    btnSaveAll.IsEnabled = true;
                }
            }
        }

        List<SteamUser> defaultUsers;
        List<SteamUser> visibleDefaultUsers;
        #endregion


       
        #endregion

        public Configuration()
        {
            InitializeComponent();
            configuration = new ServerConfig();
            LoadConfigBW = new BackgroundWorker();
            SaveConfigBW = new BackgroundWorker();
            admin = new Admin();
            LoadAdminBW = new BackgroundWorker();
            SaveAdminBW = new BackgroundWorker();
            defaultUsers = new List<SteamUser>();
            visibleDefaultUsers = new List<SteamUser>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigPath = Properties.Settings.Default.ConfigPath;
            AdminPath = Properties.Settings.Default.AdminPath;
            this.configPathExits = false;
            this.adminPathExits = false;
            setConfigPathText();
            setAdminPathText();
            this.configLoaded = false;
            this.adminLoaded = false;

            #region set up configuration enums
            GameModes = Models.EnumHelper.GetAllValuesAndDescriptions<Models.GameModeOption>().ToList<KeyValuePair<Int32, String>>();
            cboGameMode.ItemsSource = GameModes;
            cboGameMode.SelectedValuePath = "Key";
            cboGameMode.DisplayMemberPath = "Value";

            DropOnDeath = Models.EnumHelper.GetAllValuesAndDescriptions<Models.DropOnDeathOption>().ToList<KeyValuePair<Int32, String>>();
            cboDropOnDeath.ItemsSource = DropOnDeath;
            cboDropOnDeath.SelectedValuePath = "Key";
            cboDropOnDeath.DisplayMemberPath = "Value";

            DropOnQuit = Models.EnumHelper.GetAllValuesAndDescriptions<Models.DropOnQuitOption>().ToList<KeyValuePair<Int32, String>>();
            cboDropOnQuit.ItemsSource = DropOnQuit;
            cboDropOnQuit.SelectedValuePath = "Key";
            cboDropOnQuit.DisplayMemberPath = "Value";

            EnemeySpawnMode = Models.EnumHelper.GetAllValuesAndDescriptions<Models.SpawnModeOption>().ToList<KeyValuePair<Int32, String>>();
            cboEnemySpawnMode.ItemsSource = EnemeySpawnMode;
            cboEnemySpawnMode.SelectedValuePath = "Key";
            cboEnemySpawnMode.DisplayMemberPath = "Value";

            CraftTimer = Models.EnumHelper.GetAllValuesAndDescriptions<Models.TimerOption>().ToList<KeyValuePair<Int32, String>>();
            cboCraftTimer.ItemsSource = CraftTimer;
            cboCraftTimer.SelectedValuePath = "Key";
            cboCraftTimer.DisplayMemberPath = "Value";

            LootTimer = Models.EnumHelper.GetAllValuesAndDescriptions<Models.TimerOption>().ToList<KeyValuePair<Int32, String>>();
            cboLootTimer.ItemsSource = LootTimer;
            cboLootTimer.SelectedValuePath = "Key";
            cboLootTimer.DisplayMemberPath = "Value";

            LandClaimDecayMode = Models.EnumHelper.GetAllValuesAndDescriptions<Models.LandClaimDecayModeOption>().ToList<KeyValuePair<Int32, String>>();
            cboLandClaimDecayMode.ItemsSource = LandClaimDecayMode;
            cboLandClaimDecayMode.SelectedValuePath = "Key";
            cboLandClaimDecayMode.DisplayMemberPath = "Value";

            GameWorld = Models.EnumHelper.GetAllValuesAndDescriptions<Models.GameWorldOption>().ToList<KeyValuePair<Int32, String>>();
            cboGameWorld.ItemsSource = GameWorld;
            cboGameWorld.SelectedValuePath = "Key";
            cboGameWorld.DisplayMemberPath = "Value";
            #endregion

            #region Setup Admin Users

            defaultUsers = SteamUser.FromSteamIDs(GetDefaultUsers());


            #endregion

        }

        #region FilePaths
        #region Configuration
        private void btnBrowseConfigurationPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.DefaultExt = "xml";
            file.CheckPathExists = true;
            file.FileName = "serverconfig.xml";
            file.AddExtension = true;
            file.Filter = "(*.xml)|*.xml";
            file.ShowDialog();
            if (File.Exists(file.FileName))
            {
                ConfigPath = file.FileName;
                saveConfigurationPath();
            }
        }

        private void btnClearConfigPath_Click(object sender, RoutedEventArgs e)
        {
            ConfigPath = "";
            configPathExits = false;
            saveConfigurationPath();
            setConfigPathText();
        }

        private void setConfigPathText()
        {
            if (ConfigPath != "")
            {
                if (File.Exists(ConfigPath))
                {
                    lblConfigPath.Content = ConfigPath;
                    configPathExits = true;
                }
                else
                {
                    lblConfigPath.Content = "could not resolve path";
                    configPathExits = false;
                }
            }
            else 
            { 
                lblConfigPath.Content = "n/a";
                configPathExits = false;
            }
        }

        private void saveConfigurationPath()
        {
            Properties.Settings.Default.ConfigPath = ConfigPath;
            Properties.Settings.Default.Save();
            Helpers.MainWindow.SetStatus("Configuration Path Saved", 5000);
            setConfigPathText();
        }

        private void btnBrowseConfigAdminFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.DefaultExt = "xml";
            file.CheckPathExists = true;
            file.FileName = "serveradmin.xml";
            file.AddExtension = true;
            file.Filter = "(*.xml)|*.xml";
            file.ShowDialog();
            if (File.Exists(file.FileName))
            {
                txtAdminFileName.Text = Path.GetFileName(file.FileName);
                configuration.AdminFileName = Path.GetFileName(file.FileName);
            }
        }

        private void btnConfigCopyAdminFromPath_Click(object sender, RoutedEventArgs e)
        {
            //TODO:Replace with admin server var - I think it just has to be the file name and that it is assumed it exists along side the config file.
            txtAdminFileName.Text = Path.GetFileName(AdminPath);
            configuration.AdminFileName = Path.GetFileName(AdminPath);
        }
        #endregion

        #region Administration
        private void btnBrowseAdminPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.DefaultExt = "xml";
            file.CheckPathExists = true;
            file.FileName = "serverconfig.xml";
            file.AddExtension = true;
            file.Filter = "(*.xml)|*.xml";
            file.ShowDialog();
            if (File.Exists(file.FileName))
            {
                AdminPath = file.FileName;
                saveAdminPath();
            }
        }

        private void btnClearAdminPath_Click(object sender, RoutedEventArgs e)
        {
            AdminPath = "";
            adminPathExits = false;
            saveAdminPath();
            setAdminPathText();
        }

        private void saveAdminPath()
        {
            Properties.Settings.Default.AdminPath = AdminPath;
            Properties.Settings.Default.Save();
            Helpers.MainWindow.SetStatus("Admin Path Saved", 5000);
            setAdminPathText();
        }

        private void setAdminPathText()
        {
            if (AdminPath != "")
            {
                if (File.Exists(AdminPath))
                {
                    lblAdminPath.Content = AdminPath;
                    this.adminPathExits = true;
                }
                else
                {
                    lblAdminPath.Content = "could not resolve path";
                    this.adminPathExits = false;
                }
            }
            else
            {
                lblAdminPath.Content = "n/a";
                this.adminPathExits = false;
            }
        }
        #endregion
        #endregion

        #region Load Files
        #region Configuration
        private void btnLoadConfig_Click(object sender, RoutedEventArgs e)
        {
            loadConfigurationAsync();
        }

        /// <summary>
        /// This is called when we need to save. It sets up the background worker and fires off the RunWorkerAsync command.
        /// </summary>
        private void loadConfigurationAsync()
        {
            if (!LoadConfigBW.IsBusy)
            {
                if (File.Exists(ConfigPath))
                {
                    manageProgress(true, "Loading Configuration");
                    LoadConfigBW = new BackgroundWorker();
                    LoadConfigBW.RunWorkerCompleted += bw_RunLoadWorkerCompleted;
                    LoadConfigBW.DoWork += bw_DoLoadWork;
                    LoadConfigBW.RunWorkerAsync();
                }
                else
                {
                    Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("Please select the path to your configuration file and try again."));
                }
            }
            else
            {
                Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("Loading... Please wait."));
            }
        }

        /// <summary>
        /// Called by LoadConfigBW when RunWorkerAsync is called
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="e">Event arguments</param>
        private void bw_DoLoadWork(object sender, DoWorkEventArgs e)
        {
            DateTime sT = DateTime.Now;
            this.errors.Clear();
            configuration = Models.ServerConfig.Get(ConfigPath, out errors);
            TimeSpan ts = DateTime.Now - sT;
            int timeout = (1000 - ((int)ts.TotalMilliseconds));
            if (timeout > 0)
            {
                Thread.Sleep(timeout);
            }
        }
        
        /// <summary>
        /// This is called when the configuration file is loaded
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="e">Event arguments</param>
        private void bw_RunLoadWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            manageProgress(false);
            if (this.errors.Count > 0)
            {
                Helpers.MainWindow.ShowModal(new ErrorViewer(errors));
            }
            else
            {                
                bindConfigurationElements();
                configLoaded = true;
                if (App.AppMainWindow.StatusText == "")//Watch!! This doesn't seem to always work..
                {
                    Helpers.MainWindow.SetStatus("Configuration Loaded", 5000);
                }
            }           
        }

        private void bindConfigurationElements()
        {
            #region Server Tab
            b.BindTexbox(txtAdminFileName, "AdminFileName", configuration);
            b.BindIntegerUpDown(iudServerPort, "ServerPort", configuration);            
            b.BindCheckbox(cbServerIsPublic, "ServerIsPublic", configuration);
            b.BindTexbox(txtServerName, "ServerName", configuration, new Helpers.Validation.ValidationRules.EmptyString(), true);
            b.BindTexbox(txtServerPassword, "ServerPassword", configuration);
            b.BindIntegerUpDown(iudServerMaxPlayers, "ServerMaxPlayerCount", configuration);            
            b.BindTexbox(txtServerDescription, "ServerDescription", configuration);
            b.BindTexbox(txtServerWebURL, "ServerWebsiteURL", configuration);
            b.BindCheckbox(cbEACEnabled, "EACEnabled", configuration);
            b.BindCheckbox(cbTelnetEnabled, "TelnetEnabled", configuration);
            b.BindIntegerUpDown(iudTelnetPort, "TelnetPort", configuration);
            b.BindCheckbox(cbControlPanelEnabled, "ControlPanelEnabled", configuration);
            b.BindTexbox(txtTelnetPassword, "TelnetPassword", configuration);
            b.BindIntegerUpDown(iudControlPanelPort, "ControlPanelPort", configuration);
            b.BindTexbox(txtControlPanelPassword, "ControlPanelPassword", configuration);
            b.BindCheckbox(cbDisableNAT, "DisableNat", configuration);
            #endregion

            #region Game Tab
            b.BindComboBox(cboGameWorld, "GameWorldValue", configuration);
            b.BindTexbox(txtGameName, "GameName", configuration, new Helpers.Validation.ValidationRules.EmptyString(), true);
            b.BindIntegerUpDown(iudGameDifficulty, "GameDifficulty", configuration);
            b.BindComboBox(cboGameMode, "GameModeValue", configuration);
            b.BindIntegerUpDown(iudDayNightLength, "DayNightLength", configuration);
            b.BindCheckbox(cbBuildCreate, "BuildCreate", configuration);
            b.BindCheckbox(cbFriendlyFire, "FriendlyFire", configuration);
            b.BindCheckbox(cbPersistentPlayerProfiles, "PersistentPlayerProfiles", configuration);
            b.BindIntegerUpDown(iudPlayerSafeZoneLevel, "PlayerSafeZoneLevel", configuration);
            b.BindIntegerUpDown(iudPlayerSafeZoneHours, "PlayerSafeZoneHours", configuration);
            b.BindIntegerUpDown(iudNightPercentage, "NightPercentage", configuration);
            b.BindComboBox(cboDropOnDeath, "DropOnDeathValue", configuration);
            b.BindComboBox(cboDropOnQuit, "DropOnQuitValue", configuration);
            b.BindComboBox(cboCraftTimer, "CraftTimerValue", configuration);
            b.BindComboBox(cboLootTimer, "LootTimerValue", configuration);
            b.BindIntegerUpDown(iudEnemySenseMemory, "EnemySenseMemory", configuration);
            b.BindComboBox(cboEnemySpawnMode, "EnemySpawnModeValue", configuration);
            b.BindIntegerUpDown(iudBlockDurabilityModifier, "BlockDurabilityModifier", configuration);
            b.BindIntegerUpDown(iudLootAbundance, "LootAbundance", configuration);
            b.BindIntegerUpDown(iudLootRespawnDays, "LootRespawnDays", configuration);
            b.BindIntegerUpDown(iudMaxSpawnedZombies, "MaxSpawnedZombies", configuration);
            b.BindIntegerUpDown(iudMaxSpawnedAnimals, "MaxSpawnedAnimals", configuration);
            b.BindIntegerUpDown(iudLandClaimSize, "LandClaimSize", configuration);
            b.BindIntegerUpDown(iudLandClaimDeadZone, "LandClaimDeadZone", configuration);
            b.BindIntegerUpDown(iudLandClaimExpiryTime, "LandClaimExpiryTime", configuration);
            b.BindComboBox(cboLandClaimDecayMode, "LandClaimDecayModeValue", configuration);
            b.BindIntegerUpDown(iudLandClaimOnlineDurabilityModifier, "LandClaimOnlineDurabilityModifier", configuration);
            b.BindIntegerUpDown(iudLandClaimOfflineDurabilityModifier, "LandClaimOfflineDurabilityModifier", configuration);
            b.BindIntegerUpDown(iudAirDropFrequency, "AirDropFrequency", configuration);

            Binding gmBinding = new Binding() { Source = configuration, Path = new PropertyPath("GameModeValue"), Mode = BindingMode.TwoWay };
            cboGameMode.SetBinding(ComboBox.SelectedValueProperty, gmBinding);
            #endregion
        }
        #endregion

        #region Administration
        private void btnLoadAdmin_Click(object sender, RoutedEventArgs e)
        {
            LoadAdminAsync();
        }

        private void LoadAdminAsync()
        {
            if (!LoadAdminBW.IsBusy)
            {
                if (File.Exists(AdminPath))
                {
                    manageProgress(true, "Loading Admin");
                    LoadAdminBW = new BackgroundWorker();
                    LoadAdminBW.RunWorkerCompleted += LoadAdminBW_RunWorkerCompleted;
                    LoadAdminBW.DoWork += LoadAdminBW_DoWork;
                    LoadAdminBW.RunWorkerAsync();                    
                }
            }
        }

        private void LoadAdminBW_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime sT = DateTime.Now;
            this.errors.Clear();
            admin = Admin.Get(AdminPath, out errors);

            TimeSpan ts = DateTime.Now - sT;
            int timeout = (1000 - ((int)ts.TotalMilliseconds));
            if (timeout > 0)
            {
                Thread.Sleep(timeout);
            }
        }

        private void LoadAdminBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            manageProgress(false);
            if (this.errors.Count > 0)
            {
                Helpers.MainWindow.ShowModal(new ErrorViewer(errors));
            }
            else                     
            {
                adminLoaded = true;
                bindAdminElements();
                updateSteam();                
            }
        }

        private void bindAdminElements()
        {
            //First, lets go through all the users and make sure they are in our default user list.
            //The function call checkAddDefaultUser will iterate each of the collections
            //and return(stop processing) if the steam ID is found. If it is not found, it will add
            //it to the default collection and save.
            admin.Administration.ForEach(x => checkAddDefaultUser(x));
            admin.Moderators.ForEach(x => checkAddDefaultUser(x));
            admin.WhiteList.ForEach(x => checkAddDefaultUser(x));
            admin.BlackList.ForEach(x => checkAddDefaultUser(x));

            bindAdminLists();
        }

        private void bindAdminLists()
        {
            //Now, bind all of the list boxes to their respective collections.
            lbUsers.ItemsSource = defaultUsers;
            lbUsers.IsSynchronizedWithCurrentItem = true;

            lbAdmins.ItemsSource = admin.Administration;
            lbAdmins.IsSynchronizedWithCurrentItem = true;

            lbModerators.ItemsSource = admin.Moderators;
            lbModerators.IsSynchronizedWithCurrentItem = true;

            lbWhiteList.ItemsSource = admin.WhiteList;
            lbWhiteList.IsSynchronizedWithCurrentItem = true;

            lbBlackList.ItemsSource = admin.BlackList;
            lbBlackList.IsSynchronizedWithCurrentItem = true;
        }

        private void checkAddDefaultUser(SteamUser user)
        {                        
            foreach (SteamUser su in defaultUsers)
            {
                if (su.SteamID == user.SteamID)
                {
                    return;
                }
            }

            defaultUsers.Add(user);
            SaveDefaultUsers();
        }

        private void cbHideAssignedUsers_Click(object sender, RoutedEventArgs e)
        {
            if (cbHideAssignedUsers.IsChecked.HasValue && cbHideAssignedUsers.IsChecked.Value)
            {
                hideDefaultUsersInUse();
            }
            else if (cbHideAssignedUsers.IsChecked.HasValue && !cbHideAssignedUsers.IsChecked.Value)
            {
                returnFullUserCollection();
            }
        }

        /// <summary>
        /// This should be called after the full collection is bound to the listbox for proper filtering.
        /// </summary>
        private void hideDefaultUsersInUse()
        {

            foreach (SteamUser dsu in defaultUsers)
            {
                Boolean found = false;
                foreach (SteamUser asu in admin.Administration)
                {
                    if (asu.SteamID == dsu.SteamID)
                    {                       
                        found = true;
                        break;
                    }
                }

                if(!found)
                foreach (SteamUser msu in admin.Moderators)
                {
                    if (msu.SteamID == dsu.SteamID)
                    {                        
                        found = true;
                        break;
                    }
                }

                if (!found)
                foreach (SteamUser wsu in admin.WhiteList)
                {
                    if (wsu.SteamID == dsu.SteamID)
                    {                       
                        found = true;
                        break;
                    }
                }

                if (!found)
                foreach (SteamUser bsu in admin.BlackList)
                {
                    if (bsu.SteamID == dsu.SteamID)
                    {                       
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    visibleDefaultUsers.Add(dsu);
                }
            }
            lbUsers.ItemsSource = visibleDefaultUsers;
            lbUsers.Items.Refresh();
            
        }

        private void returnFullUserCollection()
        {
            visibleDefaultUsers = defaultUsers;
            lbUsers.ItemsSource = visibleDefaultUsers;
            lbUsers.Items.Refresh();
        }

        private void SaveDefaultUsers()
        {
            if (Properties.Settings.Default.Users != null)
            {
                Properties.Settings.Default.Users.Clear();
            }
            else
            {
                Properties.Settings.Default.Users = new System.Collections.Specialized.StringCollection();
            }
            foreach (SteamUser su in defaultUsers)
            {
                Properties.Settings.Default.Users.Add(su.SteamID);
            }
            Properties.Settings.Default.Save();
        }
       
        #endregion
        #endregion

        #region SaveFiles
        #region Configuration
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveConfigBW.IsBusy)
            {
                if (Validator.IsValid(this))
                {
                    manageProgress(true, "Saving Configuration");
                    SaveConfigBW = new BackgroundWorker();
                    SaveConfigBW.RunWorkerCompleted += saveConfigBW_RunWorkerCompleted;
                    SaveConfigBW.DoWork += saveConfigBW_DoWork;
                    SaveConfigBW.RunWorkerAsync();
                }
                else
                {
                    Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("Please correct the configuration errors before attempting to save."));
                }
            }
            else
            {
                Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("Saving...Please wait."));
            }
        }

        void saveConfigBW_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime sT = DateTime.Now;
            configuration.Save(this.ConfigPath, out errors);
            TimeSpan ts = DateTime.Now - sT;
            int timeout = (1000 - ((int)ts.TotalMilliseconds));
            if (timeout > 0)
            {
                Thread.Sleep(timeout);
            }
        }

        void saveConfigBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            manageProgress(false);
            if (this.errors.Count > 0)
            {
                Helpers.MainWindow.ShowModal(new ErrorViewer(errors));
                this.errors.Clear();
            }
            else
            {
                Helpers.MainWindow.SetStatus("Configuration Saved", 5000);
                loadConfigurationAsync();
            }            
        }
        #endregion

                

        #region Administration
        #endregion
        #endregion

        #region Steam
        private void btnRefreshSteam_Click(object sender, RoutedEventArgs e)
        {
            updateSteam();
        }

        private void updateSteam()
        {
            if (admin != null)
            {
                if (defaultUsers != null && defaultUsers.Count > 0)
                {
                    defaultUsers.ForEach(x => x.SteamUpdated += SteamUpdated);
                }

                if (admin.Administration != null && admin.Administration.Count > 0)
                {
                    admin.Administration.ForEach(x => x.SteamUpdated += SteamUpdated);
                }

                if (admin.Moderators != null && admin.Moderators.Count > 0)
                {
                    admin.Moderators.ForEach(x => x.SteamUpdated += SteamUpdated);
                }

                if (admin.WhiteList != null && admin.WhiteList.Count > 0)
                {
                    admin.WhiteList.ForEach(x => x.SteamUpdated += SteamUpdated);
                }

                if (admin.BlackList != null && admin.BlackList.Count > 0)
                {
                    admin.BlackList.ForEach(x => x.SteamUpdated += SteamUpdated);
                }
                admin.GetSteamData();
                defaultUsers.ForEach(x => x.GetSteamData());
            }
        }

        private void SteamUpdated(object sender, EventArgs e)
        {
            lbUsers.Items.Refresh();
            lbAdmins.Items.Refresh();
            lbModerators.Items.Refresh();
            lbWhiteList.Items.Refresh();
            lbBlackList.Items.Refresh();
        }
        #endregion


        private void manageProgress(Boolean show, String text = "")
        {
            if (show)
            {
                Progress.Visibility = System.Windows.Visibility.Visible;
                ProgressText.Text = text;
                ProgressText.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Progress.Visibility = System.Windows.Visibility.Hidden;
                ProgressText.Text = "";
                ProgressText.Visibility = System.Windows.Visibility.Hidden;
            }
        }        

        private void btnLoadAll_Click(object sender, RoutedEventArgs e)
        {
            if (!configPathExits && !adminPathExits)
            {
                Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("No path to load.\nPlease select a configuration and/or server admin file."));
            }
            else
            {
                if (configPathExits)
                {
                    loadConfigurationAsync();
                }

                if (adminPathExits)
                {
                    LoadAdminAsync();
                }
            }
        }

        private List<String> GetDefaultUsers()
        {
            List<String> steamIds = new List<String>();
            if (Properties.Settings.Default.Users != null)
            {
                foreach (String s in Properties.Settings.Default.Users)
                {
                    steamIds.Add(s);
                }
            }
            return steamIds;
        }

        #region DragDrop        
        private Point lbDragStartPosition;
       
        private void lbUsers_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lbDragStartPosition = e.GetPosition(null);
        }

        private void lbUsers_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = lbDragStartPosition - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem =
                    Helpers.ViewHelpers.ListElementHelper.FindVisualChild<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (listBoxItem != null)
                {
                    // Find the data behind the ListViewItem
                    SteamUser user = (SteamUser)listBox.ItemContainerGenerator.
                        ItemFromContainer(listBoxItem);

                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("SteamUser", user);
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
            }
            
        }

        private void lbAdmin_DragEnter(object sender, DragEventArgs e)
        {            
            if (!e.Data.GetDataPresent("SteamUser") || sender == e.Source)            
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void lbAdmin_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SteamUser"))
            {
                SteamUser user = e.Data.GetData("SteamUser") as SteamUser;
                ListBox lb = sender as ListBox;
                admin.Administration.Add(user);
                lb.Items.Refresh();
            }
        }

        private void lbModerators_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SteamUser"))
            {
                SteamUser user = e.Data.GetData("SteamUser") as SteamUser;
                ListBox lb = sender as ListBox;
                admin.Moderators.Add(user);
                lb.Items.Refresh();
            }
        }
        #endregion

        private void tiUsers_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                SteamUser su = null;
                if (lbUsers.SelectedIndex >= 0)
                {
                    su = lbUsers.SelectedItem as SteamUser;
                    defaultUsers.Remove(su);
                    SaveDefaultUsers();
                    lbUsers.Items.Refresh();
                }
                else if (lbAdmins.SelectedIndex >= 0)
                {
                    su = lbAdmins.SelectedItem as SteamUser;
                    admin.Administration.Remove(su);
                    lbAdmins.Items.Refresh();
                }
                else if (lbModerators.SelectedIndex >= 0)
                {
                    su = lbModerators.SelectedItem as SteamUser;
                    admin.Moderators.Remove(su);
                    lbModerators.Items.Refresh();
                }
                else if (lbWhiteList.SelectedIndex > 0)
                {
                    su = lbWhiteList.SelectedItem as SteamUser;
                    admin.WhiteList.Remove(su);
                    lbWhiteList.Items.Refresh();
                }
                else if (lbBlackList.SelectedIndex > 0)
                {
                    su = lbBlackList.SelectedItem as SteamUser;
                    admin.BlackList.Remove(su);
                    lbBlackList.Items.Refresh();
                }
            }
        }

        

        //This couple of functions work harder than you might think. 
        //When I select a box, I disable that handler, then call ClearAll...
        //Each of the boxes will have their selection changed, in-turn (I believe)
        //recalling this function in their scope. It does work as expected though.
        //Keep an eye on them.
        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = ((ListBox)sender);
            lb.SelectionChanged -= ListBoxSelectionChanged;
            String id = lb.Name;
            Int32 selectedIndex = lb.SelectedIndex;
            ClearAllListBoxSelections();
            switch(id)
            { 
                case "lbUsers":
                    lbUsers.SelectedIndex = selectedIndex;
                    break;
                case "lbAdmins":
                    lbAdmins.SelectedIndex = selectedIndex;
                    break;
                case "lbModerators":
                    lbModerators.SelectedIndex = selectedIndex;
                    break;
                case "lbWhiteList":
                    lbWhiteList.SelectedIndex = selectedIndex;
                    break;
                case "lbBlackList":
                    lbBlackList.SelectedIndex = selectedIndex;
                    break;
            }
            lb.SelectionChanged += ListBoxSelectionChanged;
        }
        private void ClearAllListBoxSelections()
        {
            lbUsers.SelectedIndex = -1;
            lbAdmins.SelectedIndex = -1;
            lbModerators.SelectedIndex = -1;
            lbWhiteList.SelectedIndex = -1;
            lbBlackList.SelectedIndex = -1;
        }

       
    }
   
}
