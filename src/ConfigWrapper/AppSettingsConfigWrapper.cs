using System;
using System.Linq;

namespace ConfigWrapper
{
    /// <summary>
    /// Simple config wrapper that draws values from app.config and web.config
    /// </summary>
    public class AppSettingsConfigWrapper : SimpleConfigWrapper, IConfigWrapper
    {
        /// <inheritdoc/>
        public override string[] AllKeys()
        {
            return System.Configuration.ConfigurationManager.AppSettings.AllKeys;
        }

        /// <inheritdoc/>
        protected override string GetValue(string key)
        {
            if (this.AllKeys().Contains(key))
            {
                return System.Configuration.ConfigurationManager.AppSettings[key];
            }

            throw new Exception(String.Format("Key {0} not found.", key));
        }

        /// <inheritdoc/>
        public string[] AllKeys(string topKey)
        {
            return System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(aa => aa.StartsWith(topKey))
                .ToArray();
        }


        /// <inheritdoc/>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key]
                .CastAsT<T>(defaultValue, separators, false);
        }

        /// <inheritdoc/>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType = false)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key]
                .CastAsT<T>(defaultValue, separators, errorOnWrongType);
        }
    }
}
