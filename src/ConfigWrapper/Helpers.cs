using System;
using System.Collections.Generic;

namespace ConfigWrapper
{
    /// <summary>
    /// helper functions for casting T appropriately
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// casts the input as T returning the default value if none is available.
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        /// <param name="input">value from config</param>
        /// <param name="defaultValue">the default</param>
        /// <returns>value or defalut, as appropriate</returns>
        public static T CastAsT<T>(this object input, T defaultValue)
        {
            if (input != null)
            {
                try
                {
                    return (T)Convert.ChangeType(input, typeof(T));
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// returns an array of T from the config value.  Arrays with no valid values return empty. 
        /// e.g. "," == []
        /// "1,2,3,4,5,," == [1,2,3,4,5]
        /// </summary>
        /// <typeparam name="T">type to return</typeparam>
        /// <param name="input">value from the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="separators">array of separator chars such as , or |</param>
        /// <returns>value from config, or default</returns>
        public static T[] CastAsT<T>(this object input, T[] defaultValue, char[] separators)
        {
            if (input != null)
            {
                var result = new List<T>();
                var arr = input.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var a in arr)
                {
                    try
                    {
                        var val = (T)Convert.ChangeType(a, typeof(T));
                        result.Add(val);
                    }
                    catch (Exception)
                    {
                        return defaultValue;
                    }
                }

                return result.ToArray();
            }

            return defaultValue;
        }
    }
}
