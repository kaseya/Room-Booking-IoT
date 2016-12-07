using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RB_IoT_App
{
    public class Settings
    {
        private StorageFolder _localFolder = ApplicationData.Current.LocalFolder;

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
    }

    class SettingsData
    {
        public string RoomName { get; set; }
    }
}
