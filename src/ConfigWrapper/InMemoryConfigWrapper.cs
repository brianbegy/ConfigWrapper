using System.Collections.Generic;
using System.Linq;

namespace ConfigWrapper
{
    /// <summary>
    /// used for in-memor situations, mainly unit tests
    /// </summary>
    public class InMemoryConfigWrapper : SimpleConfigWrapper, IWritableConfigWrapper
    {
        /// <summary>
        /// storage mechanism for in-memory ops
        /// </summary>
        private readonly Dictionary<string, string> storage = new Dictionary<string, string>();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string[] AllKeys()
        {
            return storage.Keys.ToArray();
        }

        protected override string GetValue(string key)
        {
            return storage[key];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string[] AllKeys(string topKey)
        {
            return storage.Keys.Where(aa => aa.StartsWith(topKey)).ToArray();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            if (storage.ContainsKey(key))
            {
                return storage[key].CastAsT(defaultValue, errorOnWrongType);
            }
            return defaultValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return this.Get<T>(key, defaultValue, separators, false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            if (storage.ContainsKey(key))
            {
                return storage[key].CastAsT(defaultValue, separators, errorOnWrongType);
            }
            return defaultValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Set<T>(string key, T value)
        {
            this.Set<T>(key, value, true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Set<T>(string key, T value, bool createKeyIfNeeded)
        {
            storage[key] = value.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Delete(string key)
        {
            if (this.storage.ContainsKey(key))
            {
                storage.Remove(key);
            }
        }
    }
}
