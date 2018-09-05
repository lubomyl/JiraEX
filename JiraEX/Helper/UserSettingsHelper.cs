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

        private static WritableSettingsStore _userSettingsStore;

        static UserSettingsHelper()
        {
            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public static string ReadFromUserSettings(string propertyName)
        {
            string ret = _userSettingsStore.GetString("External Tools", propertyName);

            return ret;
        }

        public static void WriteToUserSettings(string propertyName, string value)
        {
            _userSettingsStore.SetString("External Tools", propertyName, value);
        }

        public static void DeletePropertyFromUserSettings(string propertyName)
        {
            _userSettingsStore.DeleteProperty("External Tools", propertyName);
        }
    }
}
