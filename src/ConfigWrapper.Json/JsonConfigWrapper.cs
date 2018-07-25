using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigWrapper.Json
{
    public class JsonConfigWrapper : SimpleConfigWrapper, IConfigWrapper
    {
        /// <summary>
        /// internal json object
        /// </summary>
        private JObject json;

        /// <summary>
        /// Constructor for the config wrapper
        /// </summary>
        /// <param name="path">path to the config file</param>
        /// <param name="encoding">encoding of the config file</param>
        public JsonConfigWrapper(string path, Encoding encoding)
        {
            json = JObject.Parse(System.IO.File.ReadAllText(path, encoding));
        }

        /// <summary>
        /// Constructor for the config wrapper
        /// </summary>
        /// <param name="path">path to an ASCII encoded json object</param>
        public JsonConfigWrapper(string path) : this(path, Encoding.ASCII)
        {
        }

        /// <summary>
        /// loads the specified config string
        /// </summary>
        /// <param name="jsonAsString">a valid json string</param>
        public void LoadJson(string jsonAsString)
        {
            json = JObject.Parse(jsonAsString);
        }

        /// <inheritdocs />
        protected override string GetValue(string key)
        {
            if (this.AllKeys().Any(aa=>aa.Equals(key, StringComparison.CurrentCultureIgnoreCase)))
            {
                var newKey = this.AllKeys().First(aa => aa.Equals(key, StringComparison.CurrentCultureIgnoreCase)); //get case-sensitive name since JSON cares
                var obj = json.SelectToken(newKey);
                return obj.ToString();
            }
            throw new Exception(String.Format("No config value found for key {0}.", key));
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return this.Get<T>(key, defaultValue, separators, false);
        }

        /// <summary>
        /// returns an array of values for the key.  If null or the wrong type, returns the default.
        /// </summary>
        /// <typeparam name="T">type of the value</typeparam>
        /// <param name="key">key in the config</param>
        /// <param name="defaultValue">default value to substitute if null</param>
        /// <param name="separators">This param is only used if the value in the json file is a string instead of an array.</param>
        /// <param name="errorOnWrongType">true = we should throw an error if the config data cannot be cast.  false = use default</param>
        /// <returns>value or default</returns>
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            string[] obj = null;
            try
            {
                if (json.ContainsKey(key))
                {
                    if (json[key] is JArray)
                    {
                        var result = new List<T>();
                        foreach (var a in ((JArray)json[key]).Select(aa => aa.Value<string>()))
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

                    return json.SelectToken(key).Value<string>().CastAsT(defaultValue, separators, errorOnWrongType);

                }
            }
            catch (Exception)
            {
                // do nothing
            }

            return obj.CastAsT<T>(defaultValue, separators, errorOnWrongType);
        }

        public override string[] AllKeys()
        {
            return json.Descendants().Where(aa => !aa.HasValues).Select(aa => System.Text.RegularExpressions.Regex.Replace(aa.Path, @"[\d]", "").Replace("[]", "")).Distinct().ToArray();
        }

        public string[] AllKeys(string topKey)
        {
            return AllKeys().Where(aa => aa.StartsWith(topKey)).ToArray();
        }

    }
}
