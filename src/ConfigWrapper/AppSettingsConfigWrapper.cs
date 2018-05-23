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
            return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue);
        }

        /// <inheritdoc/>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue, separators);
        }
    }
}
