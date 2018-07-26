using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="errorOnWrongType">true = throw an exception if the input value cannot be cast to T.  false = return default if cast fails.</param>
        /// <returns>value or defalut, as appropriate</returns>
        public static T CastAsT<T>(this object input, T defaultValue, bool errorOnWrongType = false)
        {
            if (input != null)
            {
                try
                {
                    return (T)Convert.ChangeType(input, typeof(T));
                }
                catch (Exception)
                {
                    if (errorOnWrongType)
                    {
                        throw new Exception($"Cannot cast '{input}' to type {typeof(T).Name}.");
                    }
                    else
                    {
                        return defaultValue;
                    }
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
        /// <param name="errorOnWrongType">true = throw an exception if the input value cannot be cast to T.  false = return default if cast fails.</param>
        /// <returns>value from config, or default</returns>
        public static T[] CastAsT<T>(this object input, T[] defaultValue, char[] separators, bool errorOnWrongType = false)
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
                        if (errorOnWrongType)
                        {
                            throw new Exception($"Cannot cast {a} to type {typeof(T).Name}.");
                        }
                        else
                        {
                            return defaultValue;
                        }
                    }
                }

                return result.ToArray();
            }
            return defaultValue;
        }

        /// <summary>
        /// Overloads .Contains with string comparison
        /// </summary>
        /// <param name="input">the array to test</param>
        /// <param name="value">the value you seek</param>
        /// <param name="c">how to compare</param>
        /// <returns></returns>
        public static bool Contains(this string[] input, string value, StringComparison c)
        {
            return input.Any(aa => aa.Equals(value, c));
        }
    }
}
