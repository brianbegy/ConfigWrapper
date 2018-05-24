using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigWrapper
{
    /// <summary>
    /// represents a config we can read and write to such as storing the config in the registry, or as a file.
    /// </summary>
    public interface IWritableConfigWrapper : IConfigWrapper
    {
        /// <summary>
        /// Writes the config value
        /// </summary>
        /// <typeparam name="T">Data type to write, must support ToString()</typeparam>
        /// <param name="key">key to use</param>
        /// <param name="value">value to write</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Writes the config value
        /// </summary>
        /// <typeparam name="T">Data type to write, must support ToString()</typeparam>
        /// <param name="key">key to use</param>
        /// <param name="value">value to write</param>
        /// <param name="createKeyIfNeeded">create the key if it doesn't exist</param>
        void Set<T>(string key, T value, bool createKeyIfNeeded);

        /// <summary>
        /// Removes the key from the config
        /// </summary>
        /// <param name="key">key to remove</param>
        void Delete(string key);
    }
}
