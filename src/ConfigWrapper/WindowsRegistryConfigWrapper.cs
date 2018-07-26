using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace ConfigWrapper
{
    /// <summary>
    /// Configuration wrapper that uses the Windows registry to store values
    /// </summary>
    public class WindowsRegistryConfigWrapper : SimpleConfigWrapper, IConfigWrapper, IWritableConfigWrapper
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

        /// <summary>
        /// such as  HLKM/MySoftware
        /// </summary>
        public string RootKey { get; private set; }

        /// <summary>
        /// Creates a new WindowsRegistryConfigWrapper 
        /// </summary>
        /// <param name="rootKey">what is the RootKey key we wish to use? e.g. hklm/software/myapplication</param>
        public WindowsRegistryConfigWrapper(string rootKey)
        {
            //we need to make the user input match how the registry names keys.
            var tmp = rootKey.Replace("/", "\\");
            var splitTmp = tmp.Split(new[] { '\\' });
            if (HKCU.Contains(splitTmp[0].ToLowerInvariant()))
            {
                splitTmp[0] = "HKEY_CURRENT_USER";
            }

            if (HKCC.Contains(splitTmp[0].ToLowerInvariant()))
            {
                splitTmp[0] = "HKEY_CURRENT_CONFIG";
            }

            if (HKLM.Contains(splitTmp[0].ToLowerInvariant()))
            {
                splitTmp[0] = "HKEY_LOCAL_MACHINE";
            }

            var rk = GetTopKey(splitTmp[0]);
            for (int i = 1; i < splitTmp.Length; i++)
            {
                if (!rk.GetSubKeyNames().Contains(splitTmp[0]))
                {
                    rk = rk.CreateSubKey(splitTmp[i]);
                }
                else
                {
                    rk = rk.OpenSubKey(splitTmp[i]);
                }
            }
            var top = GetParentKey(splitTmp[0], true, true);
            RootKey = String.Join("\\", splitTmp);
        }

       /// <inheritdocs/>
        public override string[] AllKeys()
        {
            var top = this.GetKey(RootKey);
            return this.GetChildren(top);
        }

        /// <inheritdocs/>
        protected override string GetValue(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new Exception(String.Format("No config value found for key {0}.", key));
            }
            // the default value below will never be used since we already ensured the key is present and have error on wrong type = true.
            string value = null;
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last()).ToString();
                regKey.Close();
            }

            return value;
        }

        /// <inheritdoc />
        public string[] AllKeys(string topKey)
        {
            var top = this.GetKey(RootKey + topKey);
            return this.GetChildren(top);
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
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            object value = null;
            key = FullKey(key);
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
            key = FullKey(key);
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last());
                regKey.Close();
            }

            return value.CastAsT<T[]>(defaultValue, errorOnWrongType);
        }

        /// <summary>
        /// returns the value for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="errorOnWrongType">true = we should throw an error if the config data cannot be cast.  false = use default</param>
        /// <param name="keyIsFullyQualified">Is the key like HKLM/Software/MyApplication/MyKey or just MyKey?</param>
        /// <returns>value or default</returns>
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType, bool keyIsFullyQualified)
        {
            object value = null;
            if (!keyIsFullyQualified)
            {
                key = FullKey(key);
            }
            using (var regKey = this.GetParentKey(key, false, false))
            {
                value = regKey.GetValue(this.GetKeyAsArray(key).Last());
                regKey.Close();
            }

            return value.CastAsT<T>(defaultValue, errorOnWrongType);
        }

        /// <inheritdoc cref="IConfigWrapper" />
        public override T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            key = FullKey(key);
            return base.Get<T>(key, defaultValue, errorOnWrongType);
        }

        /// <inheritdoc cref="IConfigWrapper"/>.
        public override T Get<T>(string key)
        {
            key = FullKey(key);
            return base.Get<T>(key);
        }

        /// <inheritdoc cref="IConfigWrapper"/>.
        public void Set<T>(string key, T value, bool createKeyIfNeeded)
        {
            key = FullKey(key);
            this.Set(key,value,createKeyIfNeeded,true);
        }


        /// <summary>
        /// Sets the key
        /// </summary>
        /// <typeparam name="T">the type of the value</typeparam>
        /// <param name="key">the key to set</param>
        /// <param name="value">the value to set</param>
        /// <param name="createKeyIfNeeded">if no such key exists, create it or just throw an exception</param>
        /// <param name="keyIsFullyQualified">is the key name fully qualified or is it a sub-key of the rootKey</param>
        public void Set<T>(string key, T value, bool createKeyIfNeeded, bool keyIsFullyQualified)
        {
            using (var regKey = this.GetParentKey(key, createKeyIfNeeded, true))
            {
                regKey.SetValue(this.GetKeyAsArray(key).Last(), value.ToString());
                regKey.Close();
            }
        }

        /// <inheritdoc cref="IWritableConfigWrapper"/>.
        public void Set<T>(string key, T value)
        {
            this.Set<T>(key, value, false);
        }

        /// <inheritdoc cref="IWritableConfigWrapper"/>.
        public void Delete(string key)
        {
            this.Delete(key, false);
        }

        /// <summary>
        /// Deletes the key
        /// </summary>
        /// <param name="key">the key name</param>
        /// <param name="isFullyQualified">is this fully qualified.  e.g. does it include the RootKey node specified when you created the registry wrapper?</param>
        public void Delete(string key, bool isFullyQualified)
        {
            if (!isFullyQualified)
            {
                key = FullKey(key);
            }
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

        /// <inheritdoc cref="IConfigWrapper"/>.
        public override bool ContainsKey(string key)
        {
            return base.ContainsKey(FullKey(key));
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
        /// <param name="key">HKLM/MYKey/ etc.</param>
        /// <returns>array of strings</returns>
        private string[] GetKeyAsArray(string key)
        {
            return key.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string FullKey(string key)
        {
            if (key.StartsWith(RootKey))
            {
                return key.Replace(@"\\", @"\");
            }

            var combined = $"{RootKey}\\{key}".Replace("/", "\\");
            while (combined.Contains(@"\\"))
            {
                combined = combined.Replace(@"\\", @"\");
            }

            return combined;
        }
    }
}
