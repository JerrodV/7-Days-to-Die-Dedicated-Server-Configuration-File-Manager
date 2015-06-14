using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace SevenDaysConfigUI.Models
{
    //This document is a little complicated...

    //When we get data from Steam, we will call back with this event type.
    public delegate void SteamDataUpdatedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// <para>The Admin object is comprised of several collections.</para>
    /// <para>Administration</para>
    /// <para>Moderators</para>
    /// <para>WhiteList</para>
    /// <para>BlackList</para>
    /// <para>Permissions</para>
    /// <para>It also contains an instance of the XML document we are working on, when loaded.</para>
    /// <para>It contains several functions:</para>
    /// <para>Get, which performs the xml parsing</para>
    /// <para>and GetSteamData, which iterated the colletions containing Steam users calls GetSteamData on each item.</para>
    /// </summary>
    public class Admin : INotifyPropertyChanged
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
        private List<SteamUser> _Administration = new List<SteamUser>();
        public List<SteamUser> Administration 
        {
            get
            {
                return _Administration;
            }
            set
            {
                _Administration = value;
                NotifyPropertyChanged();
            }
        }

        private List<SteamUser> _Moderators = new List<SteamUser>();
        public List<SteamUser> Moderators 
        {
            get
            {
                return _Moderators;
            }
            set
            {
                _Moderators = value;
                NotifyPropertyChanged();
            }
        }

        private List<SteamUser> _WhiteList = new List<SteamUser>();
        public List<SteamUser> WhiteList  
        {
            get
            {
                return _WhiteList;
            }
            set
            {
                _WhiteList = value;
                NotifyPropertyChanged();
            }
        }

        private List<SteamUser> _BlackList = new List<SteamUser>();
        public List<SteamUser> BlackList  
        {
            get
            {
                return _BlackList;
            }
            set
            {
                _BlackList = value;
                NotifyPropertyChanged();
            }
        }

        private List<UserPermission> _Permissions = new List<UserPermission>(); 
        public List<UserPermission> Permissions  
        {
            get
            {
                return _Permissions;
            }
            set
            {
                _Permissions = value;
                NotifyPropertyChanged();
            }
        }

        private List<UserPermission> _DefaultPermissions = new List<UserPermission>();
        public List<UserPermission> DefaultPermissions
        {
            get
            {
                return _DefaultPermissions;
            }
        }


        public XmlDocument Document { get; set; }        

        public static Admin Get(String path, out List<Exception> errors)
        {
            Admin retVal = new Admin();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            errors = new List<Exception>();
            try
            {
                doc.Load(path);
                XmlNode baseNode = doc.SelectSingleNode("adminTools");

                XmlNodeList admins = baseNode.SelectNodes("admins/admin");
                if (admins != null)
                {
                    foreach (XmlNode node in admins)
                    {
                        dynamic d = new ExpandoObject();
                        d.Name = node.Attributes["steamID"].Value;
                        d.Value = node.Attributes["permission_level"].Value;
                        retVal.Administration.Add(new SteamUser(d));
                    }
                }

                XmlNodeList mods = baseNode.SelectNodes("moderators/moderator");
                if (mods != null)
                {
                    foreach (XmlNode node in mods)
                    {
                        dynamic d = new ExpandoObject();
                        d.Name = node.Attributes["steamID"].Value;
                        d.Value = node.Attributes["permission_level"].Value;
                        retVal.Moderators.Add(new SteamUser(d));
                    } 
                }

                XmlNodeList perms = baseNode.SelectNodes("permissions/permission");
                if (perms != null)
                {
                    foreach (XmlNode node in perms)
                    {
                        dynamic d = new ExpandoObject();
                        d.Name = node.Attributes["cmd"].Value;
                        d.Value = node.Attributes["permission_level"].Value;
                        retVal.Permissions.Add(new UserPermission(d));
                    } 
                }

                XmlNodeList wList = baseNode.SelectNodes("whitelist/whitelisted");
                if (wList != null)
                {
                    foreach (XmlNode node in wList)
                    {
                        dynamic d = new ExpandoObject();
                        d.Name = node.Attributes["steamID"].Value;
                        d.Value = node.Attributes["permission_level"].Value;
                        retVal.WhiteList.Add(new SteamUser(d));
                    } 
                }

                XmlNodeList bList = baseNode.SelectNodes("blacklist/blacklisted");
                if (bList != null)
                {
                    foreach (XmlNode node in bList)
                    {
                        dynamic d = new ExpandoObject();
                        d.Name = node.Attributes["steamID"].Value;
                        d.Value = "not int parsable";
                        d.UnBanDate = node.Attributes["unbandate"].Value;
                        retVal.BlackList.Add(new SteamUser(d));
                    } 
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
           
            retVal.Document = doc;
            return retVal;
        }

        public void GetSteamData()
        {
            Administration.ForEach(x => x.GetSteamData());
            Moderators.ForEach(x => x.GetSteamData());
            WhiteList.ForEach(x => x.GetSteamData());
            BlackList.ForEach(x => x.GetSteamData());
        }

        public void Save(String path, out List<Exception> errors) 
        {
            errors = new List<Exception>();
            
            XmlDocument doc = this.Document;
            doc.PreserveWhitespace = false;
            XmlNode baseNode = doc.DocumentElement;
            XmlNodeList nodes = baseNode.SelectNodes("*");

            foreach (XmlNode node in nodes)
            {
                switch (node.Name)
                { 
                    case "admins":
                        try
                        {
                            XmlNodeList curA = node.SelectNodes("admin");
                            foreach(XmlNode _n in curA)
                            {
                                node.RemoveChild(_n);
                            }
                            node.AppendChild(doc.CreateTextNode("\n\t\t"));
                            foreach (SteamUser u in this.Administration)
                            {
                                XmlNode n = Document.CreateNode("element", "admin", "");
                                XmlAttribute id = Document.CreateAttribute("attribute", "steamID", "");
                                id.Value = u.SteamID;
                                n.Attributes.Append(id);

                                XmlAttribute permission_level = Document.CreateAttribute("attribute", "permission_level", "");
                                permission_level.Value = u.PermissionLevel.ToString();
                                n.Attributes.Append(permission_level);
                                node.AppendChild(n);
                                node.AppendChild(doc.CreateTextNode("\n"));
                                if ((this.Administration.IndexOf(u) + 1) != this.Administration.Count)
                                {
                                    node.AppendChild(doc.CreateTextNode("\t\t"));
                                }
                                else
                                {
                                    node.AppendChild(doc.CreateTextNode("\t"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                        break;
                    case "moderators":
                        try
                        {
                            XmlNodeList curM = node.SelectNodes("moderator");
                            foreach (XmlNode _n in curM)
                            {
                                node.RemoveChild(_n);
                            }
                            node.AppendChild(doc.CreateTextNode("\n\t\t"));
                            foreach (SteamUser u in this.Moderators)
                            {
                                XmlNode n = Document.CreateNode("element", "moderator", "");
                                XmlAttribute id = Document.CreateAttribute("attribute", "steamID", "");
                                id.Value = u.SteamID;
                                n.Attributes.Append(id);

                                XmlAttribute permission_level = Document.CreateAttribute("attribute", "permission_level", "");
                                permission_level.Value = u.PermissionLevel.ToString();
                                n.Attributes.Append(permission_level);
                                node.AppendChild(n);
                                node.AppendChild(doc.CreateTextNode("\n"));
                                if (this.Moderators.IndexOf(u) != this.Moderators.Count - 1)
                                {
                                    node.AppendChild(doc.CreateTextNode("\t\t"));
                                }
                                else
                                {
                                    node.AppendChild(doc.CreateTextNode("\t"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                        break;
                    case "permissions":
                        try
                        {
                            XmlNodeList curP = node.SelectNodes("permission");
                            foreach (XmlNode _n in curP)
                            {
                                node.RemoveChild(_n);
                            }                            
                            node.AppendChild(doc.CreateTextNode("\n\t\t"));
                            foreach (UserPermission u in this.Permissions)
                            {
                                XmlNode n = Document.CreateNode("element", "permission", "");
                                XmlAttribute cmd = Document.CreateAttribute("attribute", "cmd", "");
                                cmd.Value = u.Command;
                                n.Attributes.Append(cmd);

                                XmlAttribute permission_level = Document.CreateAttribute("attribute", "permission_level", "");
                                permission_level.Value = u.PermissionLevel.ToString();
                                n.Attributes.Append(permission_level);
                                node.AppendChild(n);
                                node.AppendChild(doc.CreateTextNode("\n"));
                                if (this.Permissions.IndexOf(u) != this.Permissions.Count - 1)
                                {
                                    node.AppendChild(doc.CreateTextNode("\t\t"));
                                }
                                else
                                {
                                    node.AppendChild(doc.CreateTextNode("\t"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                        break;
                    case "whitelist":
                        try
                        {
                            XmlNodeList curW = node.SelectNodes("whitelisted");
                            foreach (XmlNode _n in curW)
                            {
                                node.RemoveChild(_n);
                            }
                            node.AppendChild(doc.CreateTextNode("\n\t\t"));
                            foreach (SteamUser u in this.WhiteList)
                            {
                                XmlNode n = Document.CreateNode("element", "whitelisted", "");
                                XmlAttribute id = Document.CreateAttribute("attribute", "steamID", "");
                                id.Value = u.SteamID;
                                n.Attributes.Append(id);

                                XmlAttribute permission_level = Document.CreateAttribute("attribute", "permission_level", "");
                                permission_level.Value = u.PermissionLevel.ToString();
                                n.Attributes.Append(permission_level);
                                node.AppendChild(n);
                                node.AppendChild(doc.CreateTextNode("\n"));
                                if (this.WhiteList.IndexOf(u) != this.WhiteList.Count - 1)
                                {
                                    node.AppendChild(doc.CreateTextNode("\t\t"));
                                }
                                else
                                {
                                    node.AppendChild(doc.CreateTextNode("\t"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                        break;
                    case "blacklist":
                        try
                        {
                            XmlNodeList curB = node.SelectNodes("blacklisted");
                            foreach (XmlNode _n in curB)
                            {
                                node.RemoveChild(_n);
                            }
                            node.AppendChild(doc.CreateTextNode("\n\t\t"));
                            foreach (SteamUser u in this.BlackList)
                            {
                                XmlNode n = Document.CreateNode("element", "blacklisted", "");
                                XmlAttribute id = Document.CreateAttribute("attribute", "steamID", "");
                                id.Value = u.SteamID;
                                n.Attributes.Append(id);
                                XmlAttribute unBanDate = Document.CreateAttribute("attribute", "unbandate", "");
                                unBanDate.Value = u.UnBanDate.Value.ToString("M/d/yyyy hh:mm tt");
                                n.Attributes.Append(unBanDate);
                                node.AppendChild(n);
                                node.AppendChild(doc.CreateTextNode("\n"));
                                if (this.BlackList.IndexOf(u) != this.BlackList.Count - 1)
                                {
                                    node.AppendChild(doc.CreateTextNode("\t\t"));
                                }
                                else
                                {
                                    node.AppendChild(doc.CreateTextNode("\t"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                        break;
                }               
            }
            doc.Save(path);
        }
    }

    /// <summary>
    /// The SteamUser will represent any Admin field where a person is indicated.
    /// See the inner comments for more information
    /// </summary>
    public class SteamUser : INotifyPropertyChanged
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

        private String _SteamID;
        public String SteamID
        {
            get 
            { 
                return _SteamID;
            }
            set 
            {
                _SteamID = value;
                NotifyPropertyChanged();
            }
        }

        private Int32 _PermissionLevel;
        public Int32 PermissionLevel
        {
            get
            {
                return _PermissionLevel;
            }
            set
            {
                _PermissionLevel = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? _UnBanDate;
        public DateTime? UnBanDate
        {
            get
            {
                return _UnBanDate;
            }

            set
            {
                _UnBanDate = value;
                NotifyPropertyChanged();
            }
        }
        
        private String _PersonaName;
        public String PersonaName
        {
            get
            {
                return _PersonaName;
            }
            set
            {
                _PersonaName = value;
                NotifyPropertyChanged();
            }
        }
        
        private String _ProfileUrl;
        public String ProfileUrl
        {
            get
            {
                return _ProfileUrl;
            }
            set
            {
                _ProfileUrl = value;
                NotifyPropertyChanged();
            }
        }
        
        private String _AvatarUrl;
        public String AvatarUrl
        {
            get 
            {
                return _AvatarUrl;
            }
            set 
            {
                _AvatarUrl = value;
                NotifyPropertyChanged();
            }
        }

        public SteamUser() { }
    
        /// <summary>
        /// This is used when creating SteamUsers from XMl
        /// </summary>
        /// <param name="data">dynamic containing Name/Value with SteamID/PermissionLevel</param>
        public SteamUser(dynamic data)
        {
            this.SteamID = data.Name.ToString();
            Int32 val;
            if (Int32.TryParse(data.Value, out val))//If This is a balck list user, Value won't parse
            {
                this.PermissionLevel = val;
            }
            else
            {
                DateTime? v;
                DateTime test;
                if (DateTime.TryParse(data.UnBanDate, out test))
                {
                    v = test;
                }
                else
                {
                    v = new DateTime?();
                }
                this.UnBanDate = v;
            }
        }

        public SteamUser(String steamID)
        {            
            this.SteamID = steamID;            
        }

        public static List<SteamUser> FromSteamIDs(List<String> ids)
        { 
            List<SteamUser> retList =  new List<SteamUser>();
            ids.ForEach(x => retList.Add(new SteamUser(x)));
            return retList;
        }

        //We will call for our Steam info on this.
        private WebClient wc = new WebClient();

        /// <summary>
        /// Creates an HTTP request to Steam for details on the user
        /// </summary>
        public void GetSteamData()
        {
            wc = new WebClient();
            Uri uri = new Uri(String.Format(Properties.Settings.Default.SteamApiUrl,Properties.Settings.Default.SteamAPIKey, this.SteamID));
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(uri);            
        }

        //This is called from the next function and inturn, firest the deligate.
        public event SteamDataUpdatedEventHandler SteamUpdated;

        // We will call this from Invoke when a steam item updates
        protected virtual void OnSteamUpdated(EventArgs e)
        {
            if (SteamUpdated != null)
            {
                SteamUpdated(this, e);
            }
        }

        //When we recieve a responce to our HTTP request, We will conver thte data to a dynamic and push it back to the main thread.
        //Then we will call the OnSteamUpdated event so we can refresh the collection on the UI.
        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Before we pass along our pointer, lets make sure it is there..
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(e.Result.ToString());
                try
                {
                    dynamic t = data.response.players.player[0];                   
                }
                catch (Exception) {
                    return;
                }
                Application.Current.Dispatcher.Invoke((Action)delegate()
                {
                    UpdateFromWebRequest(data.response.players.player[0]);
                    OnSteamUpdated(EventArgs.Empty);
                }, null);
            }
        }

        //We needed to call out of the secondary thread back into the main thread to set our pointers.(or so I descovered)
        //So, this will finish up navigating to our data and setting the pointers.
        private void UpdateFromWebRequest(dynamic player)
        {           
            this.PersonaName = player.personaname.ToString();
            this.ProfileUrl = player.profileurl.ToString();
            this.AvatarUrl = player.avatarfull.ToString();
        }
    }

    public class UserPermission : INotifyPropertyChanged
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

        public UserPermission(String Command, Int32 PermissionLevel, String PermissionDescription)
        {
            this.Command = Command;
            this.PermissionLevel = PermissionLevel;
            this.PermissionDescription = PermissionDescription;
        }

        private String _Command;
        public String Command 
        {
            get
            {
                return _Command;
            }
            private set
            {
                _Command = value;
            } 
        }

        private Int32? _PermissionLevel;
        public Int32? PermissionLevel 
        {
            get
            {
                return _PermissionLevel;
            }
            set
            {
                _PermissionLevel = value;
            }
        }

        private String _PermissionDescription;
        public String PermissionDescription
        {
            get
            {
                return _PermissionDescription;
            }

            set 
            {
                _PermissionDescription = value;
            }
        }

        public UserPermission(dynamic data) 
        {
            this.Command = data.Name.ToString();
            this.PermissionLevel = Convert.ToInt32(data.Value);
        }

        public void SetDefault(String cmd)
        {
            this.Command = cmd;
            this.PermissionLevel = 9999;
        }

        public static List<UserPermission> DefaultPermissions
        {
            get {
                return new List<UserPermission>() { 
                    new UserPermission("dm", -1, "Toggles debug menu on or off (for developers)."),
                    new UserPermission("se", -1, "Shows a list of entities that can be spawned."),
                    new UserPermission("mem", -1, "Prints memory information and calls garbage collector."),
                    new UserPermission("admin", -1, "Used to add/remove/update a player to the admin list with the desired permission level."),
                    new UserPermission("mod", -1, "Used to add/remove/update a player to the moderators list with the desired permission level."),
                    new UserPermission("cp", -1, "Used to add/remove/update a command to the command permission list with the desired permission level."),
                    new UserPermission("say", -1, "Sends a server message to all connected clients."),
                    new UserPermission("shutdown", -1, "Shuts the game down."),
                    new UserPermission("st", -1, "Sets the current world time.\nHour notation is military time * 1000 (1000 = 1 hour).\neg. 0 = Day 1, 8h; 8000 = Day 1, 16h; 16000 = Day 2, 0h; 24000 = Day 2, 08h"),
                    new UserPermission("le", -1, "Lists all entities currently in game."),
                    new UserPermission("cc", -1, "Shows all loaded chunks in the cache."),
                    new UserPermission("kick", -1, "Kicks a player from the game, reason is optional."),
                    new UserPermission("ban", -1, "Bans a player from the game for the timeframe selected,\n allowed timeframes are minutes, hours, days, weeks, months, and years.\ne.g \"ban 175 10 hours\" would apply a 10 hour ban to the playerID 175."),
                    new UserPermission("lp", -1, "Lists all players currently in game.")
                };
            }
        }
    }
}
