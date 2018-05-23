using System;

namespace ConfigWrapper
{
    public static class Helpers
    {
        /// <summary>
        /// casts the input as T returning the default value if none is available.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T CastAsT<T>(this object input, T defaultValue) {
            if (input != null)
            {
                if (input is T)
                {
                    return (T)Convert.ChangeType(input, typeof(T));
                }
            }
            return defaultValue;
        }
    }
}
