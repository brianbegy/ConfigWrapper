﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigWrapper
{
    /// <summary>
    /// abstract class that supports simple data stores such as dictionaries. 
    /// </summary>
    public abstract class SimpleConfigWrapper
    {
        /// <summary>
        /// returns all the keys
        /// </summary>
        /// <returns>and array of strings.</returns>
        public abstract string[] AllKeys();

        /// <summary>
        /// Gets the raw value as a string
        /// </summary>
        /// <param name="key">the key to lookup</param>
        /// <returns>the value as a string</returns>
        protected abstract string GetValue(string key);

        /// <summary>
        /// returns the value for the key.  If the key is missing, it throws an exception
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="separators">how to separate the array</param>
        public virtual T[] Get<T>(string key, char[] separators)
        {
            if (!this.ContainsKey(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.GetValue(key).CastAsT<T>(new List<T>().ToArray(), separators, true);
        }

        /// <summary>
        /// returns the value for the key.  If the key is missing, it throws an exception
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        public virtual T Get<T>(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.GetValue(key).CastAsT<T>(default(T), true);
        }

        ///  <inheritdoc cref="IConfigWrapper" />
        public virtual T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        ///  <inheritdoc cref="IConfigWrapper" />
        public virtual T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            if (this.ContainsKey(key))
            {
                return this.GetValue(key).CastAsT<T>(defaultValue, errorOnWrongType);
            }
            return defaultValue;
        }

        ///  <inheritdoc cref="IConfigWrapper" />
        public virtual bool ContainsKey(string key)
        {
            return this.AllKeys().Contains(key, StringComparison.CurrentCultureIgnoreCase);
        }

    }
}
