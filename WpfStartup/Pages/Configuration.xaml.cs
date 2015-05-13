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
using b = SevenDaysConfigUI.Helpers.Bindings;

namespace SevenDaysConfigUI.Pages
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Page
    {
        #region Local Vars
        #region Configuration
        BackgroundWorker LoadConfigBW;
        BackgroundWorker SaveConfigBW;
        String ConfigPath = "";
        ServerConfig configuration;
        List<Exception> errors = new List<Exception>();

        List<KeyValuePair<Int32, String>> GameModes;
        List<KeyValuePair<Int32, String>> DropOnDeath;
        List<KeyValuePair<Int32, String>> DropOnQuit;
        List<KeyValuePair<Int32, String>> EnemeySpawnMode;
        List<KeyValuePair<Int32, String>> CraftTimer;
        List<KeyValuePair<Int32, String>> LootTimer;
        List<KeyValuePair<Int32, String>> LandClaimDecayMode;
        List<KeyValuePair<Int32, String>> GameWorld;

        Boolean _configLoaded;
        Boolean configLoaded
        {
            get
            {
                return _configLoaded;
            }

            set 
            {
                btnSaveConfig.IsEnabled = value;
                _configLoaded = value;
            }
        }
        #endregion

        #endregion

        public Configuration()
        {
            InitializeComponent();
            LoadConfigBW = new BackgroundWorker();
            SaveConfigBW = new BackgroundWorker();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigPath = Properties.Settings.Default.ConfigPath;
            setConfigPathText();
            this.configLoaded = false;
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

        private void setConfigPathText()
        {
            if (ConfigPath != "")
            {
                if (File.Exists(ConfigPath))
                {
                    lblConfigPath.Content = ConfigPath;                   
                }
                else
                {
                    lblConfigPath.Content = "could not resolve path";
                }
            }
            else 
            { 
                lblConfigPath.Content = "could not resolve path"; 
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
            }
        }

        private void btnConfigCopyAdminFromPath_Click(object sender, RoutedEventArgs e)
        {
            //TODO:Replace with admin server var - I think it just has to be the file name and that it is assumed it exists along side the config file.
            txtAdminFileName.Text = Path.GetFileName("serveradmin.xml");
        }
        #endregion

        #region Administration

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
            //This out value is pur private list of errors so needs no assignment after the function call.
            configuration = Models.ServerConfig.Get(ConfigPath, out errors);
            this.errors.Clear();
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
                btnSaveConfig.IsEnabled = true;
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
        #endregion

        #region Administration
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

    }
}
