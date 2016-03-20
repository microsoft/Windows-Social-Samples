using Windows.Storage;

namespace SocialInfoSampleApp.SharedComponents
{
    public sealed class ApplicationSettings
    {
        /// <summary>
        /// The name of the setting that will be used for determining
        /// if the user is logged into the app.
        /// </summary>
        private const string cIsLoggedInSettingName = "IsUserLoggedIn";

        /// <summary>
        /// Determines if the user is logged into the app
        /// </summary>
        /// <returns>True if the user is logged in, otherwise false</returns>
        public static bool IsUserLoggedIn()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            bool isLoggedIn = false;

            // Check to see if the setting is present
            if (localSettings.Values.ContainsKey(cIsLoggedInSettingName))
            {
                 isLoggedIn = (bool)(localSettings.Values[cIsLoggedInSettingName]);
            }

            return isLoggedIn;
        }

        /// <summary>
        /// Saves the state of the user being logged into the app
        /// </summary>
        /// <param name="isUserLoggedIn">
        /// True to indicate that the user is logged in, otherwise false
        /// </param>
        public static void SaveIsUserLoggedIn(bool isUserLoggedIn)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            // Check to see if the setting is present
            if (localSettings.Values.ContainsKey(cIsLoggedInSettingName))
            {
                localSettings.Values[cIsLoggedInSettingName] = isUserLoggedIn;
            }
            else
            {
                localSettings.Values.Add(cIsLoggedInSettingName, isUserLoggedIn);
            }
        }
    }
}
