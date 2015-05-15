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

        public XmlDocument Document { get; set; }        

        public static Admin Get(String path, out List<Exception> errors)
        {
            Admin retVal = new Admin();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
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
                        d.Value = node.Attributes["permission_level"].Value;
                        retVal.WhiteList.Add(new SteamUser(d));
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
            }
        }

        /// <summary>
        /// This is used when creating SteamUsers from XMl
        /// </summary>
        /// <param name="data">dynamic containing Name/Value with SteamID/PermissionLevel</param>
        public SteamUser(dynamic data)
        {
            this.SteamID = data.Name.ToString();
            this.PermissionLevel = Convert.ToInt32(data.Value);
        }

        //We will call for our Steam info on this.
        private WebClient wc = new WebClient();

        /// <summary>
        /// Creates an HTTP request to Steam for details on the user
        /// </summary>
        public void GetSteamData()
        { 
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
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(e.Result.ToString());
                Application.Current.Dispatcher.Invoke((Action)delegate()
                {
                    UpdateFromWebRequest(data);
                    OnSteamUpdated(EventArgs.Empty);
                }, null);
            }
        }

        //We needed to call out of the secondary thread back into the main thread to set our pointers.(or so I descovered)
        //So, this will finish up navigating to our data and setting the pointers.
        private void UpdateFromWebRequest(dynamic data)
        {
            dynamic player = data.response.players.player[0];
            this.PersonaName = player.personaname.ToString();
            this.ProfileUrl = player.profileurl.ToString();
            this.AvatarUrl = player.avatar.ToString();
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
    }
}
