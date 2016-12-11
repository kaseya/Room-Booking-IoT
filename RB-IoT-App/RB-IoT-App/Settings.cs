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
        private string _defaultRoomName = "Conference Room";
        private string _defaultCompanyLogo = "Assets/CompanyLogo.png";
        private string _defaultTimeFormat = "hh:mm tt";

        private string defaultSettings = 
            "{" + System.Environment.NewLine +
                "'roomName':'Conference Room'," + System.Environment.NewLine +
                "'companyLogo':'Assets/CompanyLogo.png'," + System.Environment.NewLine +
                "'timeFormat':'hh:mm tt'" + System.Environment.NewLine +
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
                    if (String.IsNullOrEmpty(_settings.RoomName))
                    {
                        _roomName = _defaultRoomName;
                    }
                    else
                    {
                        _roomName = _settings.RoomName;
                    }
                }
                catch( Exception e)
                {
                    // TODO: Write to generic log
                    _roomName = _defaultRoomName;
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

        public string TimeFormat
        {
            get
            {
                string _timeFormat = String.Empty;

                try
                {
                    // TODO: Does .NET have a TryParse() for format strings?? If so, we should validate this input
                    _timeFormat = _settings.TimeFormat;
                    if( _timeFormat == null )
                    {
                        _timeFormat = _defaultTimeFormat;
                    }
                }
                catch (Exception)
                {
                    // TODO: Write to generic log
                    _timeFormat = _defaultTimeFormat;
                }

                return _timeFormat;
            }
        }
    }

    class SettingsData
    {
        public string RoomName { get; set; }
        public string CompanyLogo { get; set; }
        public string BackgroundImage { get; set; }
        public string TimeFormat { get; set; }
    }
}
