using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RB_IoT_App
{
    public class Settings
    {
        private StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private string _defaultCompanyLogo = "Assets/CompanyLogo.png";

        private string defaultSettings = 
            "{" + System.Environment.NewLine +
                "'roomName':'Conference Room'" + System.Environment.NewLine +
            "}";

        private SettingsData _settings;

        public Settings()
        {
            ReadSettings();
        }

        async void ReadSettings()
        {
            try
            {
                StorageFile settingsFile = await _localFolder.GetFileAsync("settings.json");
                String jsonData = await FileIO.ReadTextAsync(settingsFile);
                _settings = JsonConvert.DeserializeObject<SettingsData>(jsonData);
            }
            catch (Exception e)
            {
                // TODO: Write to generic log
                StorageFile settingsFile = await _localFolder.CreateFileAsync("settings.json");
                await FileIO.WriteTextAsync(settingsFile, defaultSettings);

                _settings = JsonConvert.DeserializeObject<SettingsData>(defaultSettings);
            }
        }

        public string RoomName
        {
            get
            {
                string _roomName = String.Empty;
                
                try
                {
                    _roomName = _settings.RoomName;
                }
                catch( Exception e)
                {
                    // TODO: Write to generic log
                    _roomName = "Conference Room";
                }

                return _roomName;
            }
            set
            {
            }
        }

        public string CompanyLogo
        {
            get
            {
                string _companyLogo = String.Empty;

                try
                {
                    FileInfo fileInfo = new FileInfo(_settings.CompanyLogo);
                    if( fileInfo != null && fileInfo.Exists)
                    {
                        _companyLogo = _settings.CompanyLogo;
                    }
                    else
                    {
                        _companyLogo = _defaultCompanyLogo;
                    }
                }
                catch( Exception )
                {
                    // TODO: Write to generic log
                    _companyLogo = _defaultCompanyLogo;

                }

                return _companyLogo;
            }
        }
    }

    class SettingsData
    {
        public string RoomName { get; set; }
        public string CompanyLogo { get; set; }
        public string BackgroundImage { get; set; }
    }
}
