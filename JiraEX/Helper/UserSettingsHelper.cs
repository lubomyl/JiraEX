using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfluenceEX.Helper
{
    public class UserSettingsHelper
    {
        private const string REGISTRY_FOLDER_NAME = "JiraEX";

        private static WritableSettingsStore _userSettingsStore;

        static UserSettingsHelper()
        {
            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!_userSettingsStore.CollectionExists(REGISTRY_FOLDER_NAME))
            {
                _userSettingsStore.CreateCollection(REGISTRY_FOLDER_NAME);
            }
        }

        public static string ReadStringFromUserSettings(string propertyName)
        {
            try
            {
                string ret = _userSettingsStore.GetString(REGISTRY_FOLDER_NAME, propertyName);

                return ret;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static bool ReadBoolFromUserSettings(string propertyName)
        {
            try
            {
                bool ret = _userSettingsStore.GetBoolean(REGISTRY_FOLDER_NAME, propertyName);

                return ret;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static void WriteToUserSettings(string propertyName, string value)
        {
            _userSettingsStore.SetString(REGISTRY_FOLDER_NAME, propertyName, value);
        }

        public static void WriteToUserSettings(string propertyName, bool value)
        {
            _userSettingsStore.SetBoolean(REGISTRY_FOLDER_NAME, propertyName, value);
        }

        public static void DeletePropertyFromUserSettings(string propertyName)
        {
            _userSettingsStore.DeleteProperty(REGISTRY_FOLDER_NAME, propertyName);
        }
    }
}
