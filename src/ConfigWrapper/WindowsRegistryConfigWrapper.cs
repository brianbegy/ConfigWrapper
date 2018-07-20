using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace ConfigWrapper
{
    /// <summary>
    /// Configuration wrapper that uses the Windows registry to store values
    /// </summary>
    public class WindowsRegistryConfigWrapper : IConfigWrapper, IWritableConfigWrapper
    {
        /// <summary>
        /// acceptable names for HKLM
        /// </summary>
        private static readonly string[] HKLM = new[] { "hklm", "hkey_local_machine" };

        /// <summary>
        /// acceptable names for HKCU
        /// </summary>
        private static readonly string[] HKCU = new[] { "hkcu", "hkey_current_user" };

        /// <summary>
        /// acceptable names for HKCC
        /// </summary>
        private static readonly string[] HKCC = new[] { "hkcc", "hkey_current_config" };

        public string[] AllKeys()
        {
            throw new NotImplementedException("The windows registry config wrapper does not support listing every registry key on the system.  That seems like a bad idea.");
        }

        /// <inheritdoc />
        public string[] AllKeys(string topKey)
        {
            var top = this.GetKey(topKey);
            return this.GetChildren(top);
        }

        public T Get<T>(string key)
        {
           
            if (!this.AllKeys(GetKeyAsArray(key)[0]).Contains(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.Get(key, default(T), true);
        }

        private RegistryKey GetKey(string topKey)
        {

            var keysAsList = this.GetKeyAsArray(topKey).ToList();
            var top = this.GetTopKey(keysAsList[0]);
            var currKey = top;
            keysAsList.RemoveAt(0);
            while (keysAsList.Any())
            {
                if (keysAsList.Count() == 1)
                {
                    if (keysAsList.Last() == currKey.Name)
                    {
                        return currKey;
                    }
                    if (currKey.GetSubKeyNames().Any(aa => aa == keysAsList.First()))
                    {
                        return currKey.OpenSubKey(keysAsList.First());
                    }
                }
                if (!currKey.GetSubKeyNames().Any(aa => aa == keysAsList.First()))
                {
                    throw new ArgumentException($"Registry key {currKey.Name} does not have a child key called {keysAsList.First()}.");
                }
                else
                {
                    currKey = currKey.OpenSubKey(keysAsList.First());
                    keysAsList.RemoveAt(0);
                }
            }
            return currKey;
        }

        private string[] GetChildren(RegistryKey key)
        {
            var result = new List<string>();
            foreach (var vals in key.GetValueNames())
            {
                result.Add($"{key.Name}\\{vals}");
            }

            foreach (var sk in key.GetSubKeyNames())
            {
                result.AddRange(this.GetChildren(key.OpenSubKey(sk)));
            }
            return result.ToArray();
        }

        private RegistryKey GetTopKey(string key)
        {
            if (HKLM.Contains(key.ToLowerInvariant()))
            {
                return Registry.LocalMachine;
            }

            if (HKCU.Contains(key.ToLowerInvariant()))
            {
                return Registry.CurrentUser;
            }

            if (HKCC.Contains(key.ToLowerInvariant()))
            {
                return Registry.CurrentConfig;
            }
            throw new NotImplementedException($"Cannot load key {key}.  Top level key must be one of: {String.Join(",", HKLM.Union(HKCU).Union(HKCC))}.");
        }

        /// <inheritdocs />
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        /// <inheritdocs />
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            object value = null;
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last());
                regKey.Close();
            }

            return value.CastAsT<T>(defaultValue, errorOnWrongType);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, char[] separators)
        {
            if (!this.AllKeys(GetKeyAsArray(key)[0]).Contains(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }

            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            return this.Get(key, new List<T>().ToArray(), separators, true);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            object value = null;
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last());
                regKey.Close();
            }

            return value.CastAsT<T>(defaultValue, separators, false);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            object value = null;
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last());
                regKey.Close();
            }

            return value.CastAsT<T[]>(defaultValue, errorOnWrongType);
        }

        /// <inheritdocs />
        public void Set<T>(string key, T value, bool createKeyIfNeeded)
        {
            using (var regKey = this.GetParentKey(key, createKeyIfNeeded, true))
            {
                regKey.SetValue(this.GetKeyAsArray(key).Last(), value.ToString());
                regKey.Close();
            }
        }

        /// <inheritdocs />
        public void Set<T>(string key, T value)
        {
            using (var regKey = this.GetParentKey(key, false, true))
            {
                regKey.SetValue(this.GetKeyAsArray(key).Last(), value.ToString());
                regKey.Close();
            }
        }

        /// <inheritdocs/>
        public void Delete(string key)
        {
            var keyAsArray = this.GetKeyAsArray(key);
            if (keyAsArray.Length == 0)
            {
                throw new Exception($"Cannot delete key '{keyAsArray.First()}");
            }

            using (var regKey = this.GetParentKey(keyAsArray.Take(keyAsArray.Length).ToArray(), false, true))
            {
                if (regKey.GetValueNames().Contains(keyAsArray.Last()))
                {
                    regKey.DeleteValue(keyAsArray.Last());
                }

                if (regKey.GetSubKeyNames().Contains(keyAsArray.Last()))
                {
                    regKey.DeleteSubKey(keyAsArray.Last());
                }
            }
        }

        /// <summary>
        /// Gets the key above the one you asked for
        /// </summary>
        /// <param name="keyPath">the key's path in the format HKLM\Foo\Bar</param>
        /// <param name="create">create the key if needed?</param>
        /// <param name="openForWrite">open the key for writing?</param>
        /// <returns>the registry key</returns>
        private Microsoft.Win32.RegistryKey GetParentKey(string keyPath, bool create, bool openForWrite = false)
        {
            return this.GetParentKey(this.GetKeyAsArray(keyPath), create, openForWrite);
        }

        /// <summary>
        /// Returns the key above the one you asked for
        /// </summary>
        /// <param name="elements">path like ["HKLM","Foo","bar"]</param>
        /// <param name="create">if it doesn't exist, create it?</param>
        /// <param name="openForWrite">open the key to write?</param>
        /// <returns>the registry key</returns>
        private RegistryKey GetParentKey(string[] elements, bool create, bool openForWrite = false)
        {
            RegistryKey key = null;

            for (int i = 0; i < elements.Length - 1; i++)
            {
                if (i == 0)
                {
                    key = this.GetTopKey(elements[0]);
                }
                else
                {
                    if (!key.GetSubKeyNames().Contains(elements[i]))
                    {
                        if (create)
                        {
                            key = key.CreateSubKey(elements[i]);
                        }
                        else
                        {
                            throw new Exception($"Key '{string.Join("/", elements)}' not found.");
                        }
                    }
                    else
                    {
                        key = key.OpenSubKey(elements[i], openForWrite);
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// Splits the key path into an array
        /// </summary>
        /// <param name="key">HKLM\MYKey\ etc.</param>
        /// <returns>array of strings</returns>
        private string[] GetKeyAsArray(string key)
        {
            return key.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
