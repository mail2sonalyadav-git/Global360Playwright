using Microsoft.Extensions.Configuration;

namespace Global360Playwright.Utils
{
    internal static class ConfigUtils
    {
        /// <summary>
        /// Loads the entire config file
        /// </summary>
        public static T Load<T>(string fileName = "settings.json")
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(PathUtils.GetConfigPath())
                .AddJsonFile(fileName, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return config.Get<T>()!;
        }

        /// <summary>
        /// Loads a section in config file.
        /// </summary>
        public static T Load<T>(string fileName = "settings.json", string sectionName = "")
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(PathUtils.GetConfigPath())
                .AddJsonFile(fileName, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return config.GetSection(sectionName).Get<T>()!;
        }
    }
}