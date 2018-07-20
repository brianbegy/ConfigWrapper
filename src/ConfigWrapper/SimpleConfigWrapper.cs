
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigWrapper
{
    public abstract class SimpleConfigWrapper
    {
        public abstract string[] AllKeys();

        protected abstract string GetValue(string key);

        /// <inheritdoc />
        public T[] Get<T>(string key, char[] separators)
        {
            if (!this.AllKeys().Contains(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.GetValue(key).CastAsT<T>(new List<T>().ToArray(), separators, true);
        }

        public T Get<T>(string key)
        {
            if (!this.AllKeys().Contains(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.GetValue(key).CastAsT<T>(default(T), true);
        }

    }
}
