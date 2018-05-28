using ConfigWrapper;
using Newtonsoft.Json.Linq;
using System.Text;

namespace JSONConfigWrapper
{
    public class JsonConfigWrapper : IConfigWrapper
    {
        /// <summary>
        /// internal json object
        /// </summary>
        private JObject json = new JObject();

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
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get(key, defaultValue, false);
        }

        /// <inheritdocs />
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            return json.SelectToken(key).CastAsT<T>(defaultValue, false);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return this.Get<T>(key, defaultValue, separators, false);
        }

        /// <inheritdocs />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            return json.SelectToken(key).CastAsT<T>(defaultValue, separators, false);
        }
    }
}
