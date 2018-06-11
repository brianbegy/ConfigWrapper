using System;

namespace ConfigWrapper
{
    /// <summary>
    /// contract for defining a configuration source
    /// </summary>
    public interface IConfigWrapper 
    {
        /// <summary>
        /// Returns all keys in the given config
        /// </summary>
        /// <returns></returns>
        string[] AllKeys();

        /// <summary>
        /// Returns all keys in the given config
        /// </summary>
        /// <returns></returns>
        string[] AllKeys(string topKey);

        /// <summary>
        /// returns the value for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <returns>value or default</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// returns the value for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="errorOnWrongType">true = we should throw an error if the config data cannot be cast.  false = use default</param>
        /// <returns>value or default</returns>
        T Get<T>(string key, T defaultValue, bool errorOnWrongType);

        /// <summary>
        /// returns an array of values for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="separators">, or | etc</param>
        /// <returns>value or default</returns>
        T[] Get<T>(string key, T[] defaultValue, char[] separators);

        /// <summary>
        /// returns an array of values for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="separators">, or | etc</param>
        /// <param name="errorOnWrongType">true = we should throw an error if the config data cannot be cast.  false = use default</param>
        /// <returns>value or default</returns>
        T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType);
    }
}
