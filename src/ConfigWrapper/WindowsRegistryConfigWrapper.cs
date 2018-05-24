using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigWrapper
{
   public  class WindowsRegistryConfigWrapper : IConfigWrapper, IWritableConfigWrapper
    {

        private string[] HKLM = new[] { "hklm", "hkey_local_machine" };
        private string[] HKCU = new[] { "hkcu", "hkey_current_user" };
        private string[] HKCC = new[] { "hkcc", "hkey_current_config" };

        /// <summary>
        /// Splits the key path into an array
        /// </summary>
        /// <param name="key">HKLM\MYKey\ etc.</param>
        /// <returns></returns>
        private string[] GetKeyAsArray(string key)
        {
            return key.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private Microsoft.Win32.RegistryKey GetParentKey(string keyPath, bool create, bool openForWrite = false)
        {
            return GetParentKey(GetKeyAsArray(keyPath), create, openForWrite);
        }

            /// <summary>
            /// Returns the key above the one you asked for
            /// </summary>
            /// <param name="elements">path like ["HKLM","Foo","bar"]</param>
            /// <param name="create">if it doesn't exist, create it?</param>
            /// <param name="openForWrite">open the key to write?</param>
            /// <returns></returns>
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
                            throw new Exception($"Key '{string.Join("/",elements)}' not found.");
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

        /// <inheritdocs />
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        /// <inheritdocs />
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            object value = null;
            using (var regKey = GetParentKey(key, false, false)) {
                value = regKey.GetValue(GetKeyAsArray(key).Last());
                regKey.Close();
            }
            return value.CastAsT<T>(defaultValue, errorOnWrongType);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            object value = null;
            using (var regKey = GetParentKey(key, false, false))
            {
                value = regKey.GetValue(GetKeyAsArray(key).Last());
                regKey.Close();
            }
            return value.CastAsT<T[]>(defaultValue);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            object value = null;
            using (var regKey = GetParentKey(key, false, false))
            {
                value = regKey.GetValue(GetKeyAsArray(key).Last());
                regKey.Close();
            }
            return value.CastAsT<T[]>(defaultValue, errorOnWrongType);
        }

        /// <inheritdocs />
        public void Set<T>(string key, T value, bool createKeyIfNeeded)
        {
            using (var regKey = GetParentKey(key, createKeyIfNeeded, true))
            {
                regKey.SetValue(GetKeyAsArray(key).Last(), value.ToString());
                regKey.Close();
            }
        }

        /// <inheritdocs />
        public void Set<T>(string key, T value)
        {
            using (var regKey = GetParentKey(key, false, true))
            {
                regKey.SetValue(GetKeyAsArray(key).Last(), value.ToString());
                regKey.Close();
            }
        }

        public void Delete(string key) {
            var keyAsArray = GetKeyAsArray(key);
            if (keyAsArray.Length == 0) {
                throw new Exception($"Cannot delete key '{keyAsArray.First()}");
            }
            using (var regKey = GetParentKey(keyAsArray.Take(keyAsArray.Length).ToArray(), false, true))
            {
                if (regKey.GetValueNames().Contains(keyAsArray.Last())) {
                    regKey.DeleteValue(keyAsArray.Last());
                }
                if (regKey.GetSubKeyNames().Contains(keyAsArray.Last())) {
                    regKey.DeleteSubKey(keyAsArray.Last());
                }
            }

        }
    }
}
