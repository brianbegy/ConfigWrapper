namespace ConfigWrapper
{
    /// <summary>
    /// contract for defining a configuration source
    /// </summary>
    public interface IConfigWrapper
    {
        /// <summary>
        /// returns the value for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <returns>value or default</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// returns an array of values for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="separators">, or | etc</param>
        /// <returns>value or default</returns>
        T[] Get<T>(string key, T[] defaultValue, char[] separators);
    }
}
