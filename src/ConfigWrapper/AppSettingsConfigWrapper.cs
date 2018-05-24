namespace ConfigWrapper
{
    /// <summary>
    /// Simple config wrapper that draws values from app.config and web.config
    /// </summary>
    public class AppSettingsConfigWrapper : IConfigWrapper
    {
        /// <inheritdoc/>
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        /// <inheritdoc/>
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType = false)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue, errorOnWrongType);
        }

        /// <inheritdoc/>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue, separators, false);
        }

        /// <inheritdoc/>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType = false)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue, separators, errorOnWrongType);
        }
    }
}
