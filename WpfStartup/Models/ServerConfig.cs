using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SevenDaysConfigUI.Models
{
    
    public enum GameWorldOption
    {
        [Description("Navezgane")]
        Navezgane,
        [Description("MP Wasteland Horde")]
        MPWastelandHorde,
        [Description("MP Wasteland Skirmish")]
        MPWastelandSkirmish,
        [Description("MP Wasteland War")]
        MPWastelandWar,
        [Description("Random Gen")]
        RandomGen

    }
    public enum GameModeOption
    {
        [Description("Survival MP")]
        GameModeSurvivalMP,
        [Description("Survival SP")]
        GameModeSurvivalSP

    }
    public enum ZombiesRunOption
    {
        [Description("Default")]
        Default = 0,
        [Description("Never Run")]
        NeverRun = 1,
        [Description("Always Run")]
        AlwaysRun = 2
    }
    public enum DropOnDeathOption
    {
        [Description("Everything")]
        Everything = 0,
        [Description("ToolBelt Only")]
        ToolBeltOnly = 1,
        [Description("Backpack Only")]
        BackPackOnly = 2,
    }
    public enum DropOnQuitOption
    {
        [Description("Nothing")]
        Nothing = 0,
        [Description("Everything")]
        Everything = 1,
        [Description("Toolbelt Only")]
        ToolbeltOnly = 2,
        [Description("Backpack Only")]
        BackpackOnly = 3
    }
    public enum TimerOption
    {
        [Description("Instant")]
        Instant = 0,
        [Description("Normal")]
        Normal = 1,
        [Description("Fast")]
        Fast = 2
    }
    public enum SpawnModeOption
    {
        [Description("Disabled")]
        Disabled = 0,
        [Description("Very Low")]
        VeryLow = 1,
        [Description("Low")]
        Low = 2,
        [Description("Medium")]
        Medium = 3,
        [Description("High")]
        High = 4,
        [Description("Very High")]
        VeryHigh = 5

    }
    public enum EnemyDifficultyOption
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Feral")]
        Feral = 1
    }
    public enum LandClaimDecayModeOption
    {
        [Description("Linear")]
        Linear = 0,
        [Description("Exponential")]
        Exponential = 1,
        [Description("Full Protection Till Expiration")]
        FullProtectionTillExpiration = 2
    }

    public class ServerConfig: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Int32 ServerPort { get; set; }
        public Boolean ServerIsPublic { get; set; }
        public String ServerName { get; set; }
        public String ServerPassword { get; set; }
        public Int32 ServerMaxPlayerCount { get; set; }
        public String ServerDescription { get; set; }
        public String ServerWebsiteURL { get; set; }

        public GameWorldOption _GameWorld;
        public GameWorldOption GameWorld 
        {
            get
            {
                return _GameWorld;
            }

            set
            {
                _GameWorld = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 GameWorldValue
        {
            get
            {
                return (Int32)_GameWorld;
            }
            set
            {
                _GameWorld = (GameWorldOption)value;
                NotifyPropertyChanged();
            }
        }

        public String GameName { get; set; }
        public Int32 GameDifficulty { get; set; }

        public GameModeOption _GameMode;
        public GameModeOption GameMode 
        {
            get 
            {
                return _GameMode;
            }
            set
            {
                _GameMode = value;
                NotifyPropertyChanged();
            }
        }        
        public Int32 GameModeValue
        {
            get 
            {
                return (Int32)_GameMode;
            }
            set 
            {
                _GameMode = (GameModeOption)value;
                NotifyPropertyChanged();
            }
        }

        public ZombiesRunOption _ZombiesRun;
        public ZombiesRunOption ZombiesRun 
        {
            get 
            {
                return _ZombiesRun;
            }
            set 
            {
                _ZombiesRun = value;
                NotifyPropertyChanged();
            } 
        }
        public Int32 ZombiesRunValue
        {
            get
            {
                return (Int32)_ZombiesRun;
            }
            set
            {
                _ZombiesRun = (ZombiesRunOption)value;
                NotifyPropertyChanged();
            }
        }

        public Boolean BuildCreate { get; set; }
        public Int32 DayNightLength  { get; set; }
        public Boolean FriendlyFire { get; set; }
        public Boolean  PersistentPlayerProfiles { get; set; }
        public Int32 PlayerSafeZoneLevel { get; set; }
        public Int32 PlayerSafeZoneHours { get; set; }
        public Boolean ControlPanelEnabled { get; set; }
        public Int32 ControlPanelPort { get; set; }
        public String ControlPanelPassword { get; set; }
        public Boolean TelnetEnabled { get; set; }
        public Int32 TelnetPort { get; set; }
        public String TelnetPassword { get; set; }
        public Boolean DisableNAT { get; set; }
        public String AdminFileName { get; set; }

        public DropOnDeathOption _DropOnDeath;
        public DropOnDeathOption DropOnDeath 
        {
            get
            {
                return _DropOnDeath;
            }

            set
            {
                _DropOnDeath = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 DropOnDeathValue
        {
            get
            {
                return (Int32)_DropOnDeath;
            }
            set
            {
                _DropOnDeath = (DropOnDeathOption)value;
                NotifyPropertyChanged();
            }
        }

        public DropOnQuitOption _DropOnQuit;
        public DropOnQuitOption DropOnQuit
        {
            get
            {
                return _DropOnQuit;
            }

            set
            {
                _DropOnQuit = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 DropOnQuitValue
        {
            get
            {
                return (Int32)_DropOnQuit;
            }
            set
            {
                _DropOnQuit = (DropOnQuitOption)value;
                NotifyPropertyChanged();
            }
        }

        public TimerOption _CraftTimer;
        public TimerOption CraftTimer 
        { 
            get
            {
                return _CraftTimer;
            }
            set
            {
                _CraftTimer = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 CraftTimerValue
        {
            get
            {
                return (Int32)_CraftTimer;
            }
            set
            {
                _CraftTimer = (TimerOption)value;
                NotifyPropertyChanged();
            }
        }

        public TimerOption _LootTimer;
        public TimerOption LootTimer
        { 
            get
            {
                return _LootTimer;
            }
            set
            {
                _LootTimer = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 LootTimerValue
        {
            get
            {
                return (Int32)_LootTimer;
            }
            set
            {
                _LootTimer = (TimerOption)value;
                NotifyPropertyChanged();
            }
        }

        public Int32 EnemySenseMemory { get; set; }

        public SpawnModeOption _EnemySpawnMode;
        public SpawnModeOption EnemySpawnMode
        {
            get
            {
                return _EnemySpawnMode;
            }
            set
            {
                _EnemySpawnMode = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 EnemySpawnModeValue
        {
            get
            {
                return (Int32)_EnemySpawnMode;
            }
            set
            {
                _EnemySpawnMode = (SpawnModeOption)value;
                NotifyPropertyChanged();
            }
        }

        public EnemyDifficultyOption _EnemyDifficulty;
        public EnemyDifficultyOption EnemyDifficulty
        {
            get
            {
                return _EnemyDifficulty;
            }
            set
            {
                _EnemyDifficulty = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 EnemyDifficultyValue
        {
            get
            {
                return (Int32)_EnemyDifficulty;
            }
            set
            {
                _EnemyDifficulty = (EnemyDifficultyOption)value;
                NotifyPropertyChanged();
            }
        }

        public Int32 NightPercentage { get; set; }
        public Int32 BlockDurabilityModifier { get; set; }
        public Int32 LootAbundance { get; set; }
        public Int32 LootRespawnDays { get; set; }
        public Int32 LandClaimSize  { get; set; }
        public Int32 LandClaimDeadZone { get; set; }
        public Int32 LandClaimExpiryTime { get; set; }

        public LandClaimDecayModeOption _LandClaimDecayMode;
        public LandClaimDecayModeOption LandClaimDecayMode
        {
            get
            {
                return _LandClaimDecayMode;
            }
            set
            {
                _LandClaimDecayMode = value;
                NotifyPropertyChanged();
            }
        }
        public Int32 LandClaimDecayModeValue
        {
            get
            {
                return (Int32)_LandClaimDecayMode;
            }
            set
            {
                _LandClaimDecayMode = (LandClaimDecayModeOption)value;
                NotifyPropertyChanged();
            }
        }

        public Int32 LandClaimOnlineDurabilityModifier { get; set; }
        public Int32 LandClaimOfflineDurabilityModifier { get; set; }
        public Int32 AirDropFrequency { get; set; }
        public Int32 MaxSpawnedZombies { get; set; }
        public Int32 MaxSpawnedAnimals { get; set; }
        public Boolean EACEnabled { get; set; }
        public XmlDocument Document { get; set; }

        public static ServerConfig Get(String path, out List<Exception> errors)
        {
            ServerConfig retVal = new ServerConfig();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            errors = new List<Exception>();
            List<dynamic> data = null;
            try
            {
                doc.Load(path);
                XmlNode baseNode = doc.SelectSingleNode("ServerSettings");
                XmlNodeList props = baseNode.SelectNodes("property");
                
                data = new List<dynamic>();
                foreach (XmlNode node in props)
                {
                    dynamic d = new ExpandoObject();
                    d.Name = node.Attributes["name"].Value;
                    d.Value = node.Attributes["value"].Value;
                    data.Add(d);
                }
            }catch(Exception ex){
                errors.Add(ex);
            }
            List<Exception> errs = new List<Exception>();
            retVal = GetConfig(data, out errs);
            errors.AddRange(errs.ToArray());
            retVal.Document = doc;
            return retVal;
        }

        private static ServerConfig GetConfig(dynamic data, out List<Exception> errors)
        {
            ServerConfig config = new ServerConfig();
            errors = new List<Exception>();
            foreach (dynamic d in data)
            { 
                String s = d.Name.ToString();
                try
                {                   
                    switch (s)
                    {
                        case "ServerPort":
                            config.ServerPort = Convert.ToInt32(d.Value);
                            break;
                        case "ServerIsPublic":
                            config.ServerIsPublic = Convert.ToBoolean(d.Value);
                            break;
                        case "ServerName":
                            config.ServerName = d.Value.ToString();
                            break;
                        case "ServerPassword":
                            config.ServerPassword = d.Value.ToString();
                            break;
                        case "ServerMaxPlayerCount":
                            config.ServerMaxPlayerCount = Convert.ToInt32(d.Value);
                            break;
                        case "ServerDescription":
                            config.ServerDescription = d.Value.ToString();
                            break;
                        case "ServerWebsiteURL":
                            config.ServerWebsiteURL = d.Value.ToString();                            
                            break;
                        case "GameWorld":
                            //This one is a little different than the rest. The value we will be handeling is the description, which is not text that is usable as an enum as it is for GameMode.
                            //This is easy to handle on the save, but here we have to jump through some hoops.

                            //First, get a list of descriptions and values of the GameWorldOption enum
                            List<KeyValuePair<Int32, String>> gwVals = Models.EnumHelper.GetAllValuesAndDescriptions<Models.GameWorldOption>().ToList<KeyValuePair<Int32, String>>();

                            //Now, look through the list to find the description (kept in the value field) for the value in the configuration
                            KeyValuePair<Int32, String> tempO = gwVals.First(x => x.Value == d.Value.ToString());

                            //Now we know the int value of the enum value that applies to the description kept in the config file.
                            //We can parse this in as a GameWorldOption and set it to our model.                            
                            config.GameWorld = (GameWorldOption)tempO.Key;
                            break;
                        case "GameName":
                            config.GameName = d.Value.ToString();
                            break;
                        case "GameDifficulty":
                            config.GameDifficulty = Convert.ToInt32(d.Value);
                            break;
                        case "GameMode":
                            config.GameMode = Enum.Parse(typeof(GameModeOption), d.Value.ToString());
                            break;
                        case "ZombiesRun":
                            config.ZombiesRun = ((ZombiesRunOption)Convert.ToInt32(d.Value));
                            break;
                        case "BuildCreate":
                            config.BuildCreate = Convert.ToBoolean(d.Value);
                            break;
                        case "DayNightLength":
                            config.DayNightLength = Convert.ToInt32(d.Value);
                            break;
                        case "FriendlyFire":
                            config.FriendlyFire = Convert.ToBoolean(d.Value);
                            break;
                        case "PersistentPlayerProfiles":
                            config.PersistentPlayerProfiles = Convert.ToBoolean(d.Value);
                            break;
                        case "PlayerSafeZoneLevel":
                            config.PlayerSafeZoneLevel = Convert.ToInt32(d.Value);
                            break;
                        case "PlayerSafeZoneHours":
                            config.PlayerSafeZoneHours = Convert.ToInt32(d.Value);
                            break;
                        case "ControlPanelEnabled":
                            config.ControlPanelEnabled = Convert.ToBoolean(d.Value);
                            break;
                        case "ControlPanelPort":
                            config.ControlPanelPort = Convert.ToInt32(d.Value);
                            break;
                        case "ControlPanelPassword":
                            config.ControlPanelPassword = d.Value.ToString();
                            break;
                        case "TelnetEnabled":
                            config.TelnetEnabled = Convert.ToBoolean(d.Value);
                            break;
                        case "TelnetPort":
                            config.TelnetPort = Convert.ToInt32(d.Value);
                            break;
                        case "TelnetPassword":
                            config.TelnetPassword = d.Value.ToString();
                            break;
                        case "DisableNAT":
                            config.DisableNAT = Convert.ToBoolean(d.Value);
                            break;
                        case "AdminFileName":
                            config.AdminFileName = d.Value.ToString();
                            break;
                        case "DropOnDeath":
                            config.DropOnDeath = ((DropOnDeathOption)Convert.ToInt32(d.Value));
                            break;
                        case "DropOnQuit":
                            config.DropOnQuit = ((DropOnQuitOption)Convert.ToInt32(d.Value));
                            break;
                        case "CraftTimer":
                            config.CraftTimer = ((TimerOption)Convert.ToInt32(d.Value));
                            break;
                        case "LootTimer":
                            config.LootTimer = ((TimerOption)Convert.ToInt32(d.Value));
                            break;
                        case "EnemySenseMemory":
                            config.EnemySenseMemory = Convert.ToInt32(d.Value);
                            break;
                        case "EnemySpawnMode":
                            config.EnemySpawnMode = ((SpawnModeOption)Convert.ToInt32(d.Value));
                            break;
                        case "EnemyDifficulty":
                            config.EnemyDifficulty = ((EnemyDifficultyOption)Convert.ToInt32(d.Value));
                            break;
                        case "NightPercentage":
                            config.NightPercentage = Convert.ToInt32(d.Value);
                            break;
                        case "BlockDurabilityModifier":
                            config.BlockDurabilityModifier = Convert.ToInt32(d.Value);
                            break;
                        case "LootAbundance":
                            config.LootAbundance = Convert.ToInt32(d.Value);
                            break;
                        case "LootRespawnDays":
                            config.LootRespawnDays = Convert.ToInt32(d.Value);
                            break;
                        case "LandClaimSize":
                            config.LandClaimSize = Convert.ToInt32(d.Value);
                            break;
                        case "LandClaimDeadZone":
                            config.LandClaimDeadZone = Convert.ToInt32(d.Value);
                            break;
                        case "LandClaimExpiryTime":
                            config.LandClaimExpiryTime = Convert.ToInt32(d.Value);
                            break;
                        case "LandClaimDecayMode":
                            config.LandClaimDecayMode = ((LandClaimDecayModeOption)Convert.ToInt32(d.Value));
                            break;
                        case "LandClaimOnlineDurabilityModifier":
                            config.LandClaimOnlineDurabilityModifier = Convert.ToInt32(d.Value);
                            break;
                        case "LandClaimOfflineDurabilityModifier":
                            config.LandClaimOfflineDurabilityModifier = Convert.ToInt32(d.Value);
                            break;
                        case "AirDropFrequency":
                            config.AirDropFrequency = Convert.ToInt32(d.Value);
                            break;
                        case "MaxSpawnedZombies":
                            config.MaxSpawnedZombies = Convert.ToInt32(d.Value);
                            break;
                        case "MaxSpawnedAnimals":
                            config.MaxSpawnedAnimals = Convert.ToInt32(d.Value);
                            break;
                        case "EACEnabled":
                            config.EACEnabled = Convert.ToBoolean(d.Value);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
            return config;
        }

        public void Save(String path, out List<Exception> errors)
        {
            errors = new List<Exception>();
            try
            {
                ServerConfig retVal = new ServerConfig();
                XmlDocument doc = this.Document;
                doc.PreserveWhitespace = true;
                XmlNode baseNode = doc.SelectSingleNode("ServerSettings");
                XmlNodeList props = baseNode.SelectNodes("property");
                foreach (XmlNode node in props)
                {
                    node.Attributes["value"].Value = GetValue(node.Attributes["name"].Value);
                }
                doc.Save(path);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }

        private String GetValue(String name)
        {
            switch (name)
            {
                case "ServerPort":
                    return this.ServerPort.ToString();                           
                        case "ServerIsPublic":
                            return this.ServerIsPublic.ToString().ToLower();
                        case "ServerName":
                            return this.ServerName.ToString();
                            
                        case "ServerPassword":
                            return this.ServerPassword.ToString();
                            
                        case "ServerMaxPlayerCount":
                            return this.ServerMaxPlayerCount.ToString().ToLower();
                            
                        case "ServerDescription":
                            return this.ServerDescription.ToString();
                            
                        case "ServerWebsiteURL":
                            return this.ServerWebsiteURL.ToString();
                            
                        case "GameWorld":
                            return this.GameWorld.Description();
                            
                        case "GameName":
                            return this.GameName.ToString();
                            
                        case "GameDifficulty":
                            return ((Int32)this.GameDifficulty).ToString();
                            
                        case "GameMode":
                            return this.GameMode.ToString();
                            
                        case "ZombiesRun":
                            return ((Int32)this.ZombiesRun).ToString();
                            
                        case "BuildCreate":
                            return this.BuildCreate.ToString().ToLower();
                            
                        case "DayNightLength":
                            return this.DayNightLength.ToString();
                            
                        case "FriendlyFire":
                            return this.FriendlyFire.ToString().ToLower();
                            
                        case "PersistentPlayerProfiles":
                            return this.PersistentPlayerProfiles.ToString().ToLower();
                            
                        case "PlayerSafeZoneLevel":
                            return this.PlayerSafeZoneLevel.ToString();
                            
                        case "PlayerSafeZoneHours":
                            return this.PlayerSafeZoneHours.ToString();
                            
                        case "ControlPanelEnabled":
                            return this.ControlPanelEnabled.ToString().ToLower();
                            
                        case "ControlPanelPort":
                            return this.ControlPanelPort.ToString();
                            
                        case "ControlPanelPassword":
                            return this.ControlPanelPassword.ToString();
                            
                        case "TelnetEnabled":
                            return this.TelnetEnabled.ToString().ToLower();
                            
                        case "TelnetPort":
                            return this.TelnetPort.ToString();
                            
                        case "TelnetPassword":
                            return this.TelnetPassword.ToString();
                            
                        case "DisableNAT":
                            return this.DisableNAT.ToString().ToLower();
                            
                        case "AdminFileName":
                            return this.AdminFileName.ToString();
                            
                        case "DropOnDeath":
                            return ((Int32)this.DropOnDeath).ToString();
                            
                        case "DropOnQuit":
                            return ((Int32)this.DropOnQuit).ToString();
                            
                        case "CraftTimer":
                            return ((Int32)this.CraftTimer).ToString();
                            
                        case "LootTimer":
                            return ((Int32)this.LootTimer).ToString().ToLower();
                            
                        case "EnemySenseMemory":
                            return this.EnemySenseMemory.ToString();
                            
                        case "EnemySpawnMode":
                            return ((Int32)this.EnemySpawnMode).ToString();
                            
                        case "EnemyDifficulty":
                            return ((Int32)this.EnemyDifficulty).ToString();
                            
                        case "NightPercentage":
                            return this.NightPercentage.ToString();
                            
                        case "BlockDurabilityModifier":
                            return this.BlockDurabilityModifier.ToString();
                            
                        case "LootAbundance":
                            return this.LootAbundance.ToString();
                            
                        case "LootRespawnDays":
                            return this.LootRespawnDays.ToString();
                            
                        case "LandClaimSize":
                            return this.LandClaimSize.ToString();
                            
                        case "LandClaimDeadZone":
                            return this.LandClaimDeadZone.ToString();
                            
                        case "LandClaimExpiryTime":
                            return this.LandClaimExpiryTime.ToString();
                            
                        case "LandClaimDecayMode":
                            return ((Int32)this.LandClaimDecayMode).ToString();
                            
                        case "LandClaimOnlineDurabilityModifier":
                            return this.LandClaimOnlineDurabilityModifier.ToString();
                            
                        case "LandClaimOfflineDurabilityModifier":
                            return this.LandClaimOfflineDurabilityModifier.ToString();
                            
                        case "AirDropFrequency":
                            return this.AirDropFrequency.ToString();
                            
                        case "MaxSpawnedZombies":
                            return this.MaxSpawnedZombies.ToString();
                            
                        case "MaxSpawnedAnimals":
                            return this.MaxSpawnedAnimals.ToString();
                            
                        case "EACEnabled":
                            return this.EACEnabled.ToString().ToLower();
                        default: throw new Exception("Name not found on data model.");
            }
        }
    }

    public static class EnumHelper
    {
        /// <summary>
        /// Gets the description of a specific enum value.
        /// </summary>
        public static string Description(this Enum eValue)
        {
            var nAttributes = eValue.GetType().GetField(eValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (nAttributes.First() as DescriptionAttribute).Description;
        }

        /// <summary>
        /// Returns an enumerable collection of all values and descriptions for an enum type.
        /// </summary>
        public static IEnumerable<KeyValuePair<Int32, string>> GetAllValuesAndDescriptions<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an Enumeration type");
            }

            return from e in Enum.GetValues(typeof(TEnum)).Cast<Enum>()
                   select new KeyValuePair<Int32, string>(Convert.ToInt32(e),  e.Description());
        }


    }
}
