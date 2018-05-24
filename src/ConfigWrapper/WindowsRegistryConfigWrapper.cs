using System;
using System.Linq;

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
        private Microsoft.Win32.RegistryKey GetParentKey(string[] elements, bool create, bool openForWrite = false)
        {
            Microsoft.Win32.RegistryKey key = null;

            for (int i = 0; i < elements.Length - 1; i++)
            {
                if (i == 0)
                {
                    if (HKLM.Contains(elements[0].ToLowerInvariant()))
                    {
                        key = Microsoft.Win32.Registry.LocalMachine;
                    }

                    if (HKCU.Contains(elements[0].ToLowerInvariant()))
                    {
                        key = Microsoft.Win32.Registry.CurrentUser;
                    }

                    if (HKCC.Contains(elements[0].ToLowerInvariant()))
                    {
                        key = Microsoft.Win32.Registry.CurrentConfig;
                    }
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
