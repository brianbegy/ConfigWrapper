using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfigWrapper
{
    /// <summary>
    /// Loads values from an ini file
    /// </summary>
    public class IniConfigWrapper : IConfigWrapper
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        private readonly string _iniPath;

        /// <summary>
        /// valid delimiters for the config file
        /// </summary>
        private readonly char[] _validDelimiters;

        /// <summary>
        /// Encoding of the config file
        /// </summary>
        private readonly Encoding _encoding;

        /// <summary>
        /// Creates an IniConfigWrapper
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <param name="encoding">encoding of the file</param>
        /// <param name="validDelimiters">e.g. '=', ';' </param>
        public IniConfigWrapper(string path, Encoding encoding, char[] validDelimiters)
        {
            this._iniPath = path;
            this._encoding = encoding;
            this._validDelimiters = validDelimiters;
        }

        /// <summary>
        /// Creates an IniConfigWrapper
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <param name="encoding">encoding of the file</param>
        public IniConfigWrapper(string path, Encoding encoding) : this(path, encoding, new[] { '=' })
        {
        }

        /// <summary>
        /// Creates an IniConfigWrapper
        /// </summary>
        /// <param name="path">path to the file</param>
        public IniConfigWrapper(string path) : this(path, Encoding.ASCII, new[] { '=' })
        {
        }
        
        /// <inheritdoc />
        public T Get<T>(string key, T defaultValue)
        {
            return this.Get<T>(key, defaultValue, false);
        }

        /// <inheritdoc />
        public T Get<T>(string key, T defaultValue, bool errorOnWrongType)
        {
            return this.GetValue(string.Empty, key).CastAsT<T>(defaultValue, errorOnWrongType);
        }

        /// <inheritdoc />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators)
        {
            return this.GetValue(string.Empty, key).CastAsT<T>(defaultValue, separators);
        }

        /// <inheritdoc />
        public T[] Get<T>(string key, T[] defaultValue, char[] separators, bool errorOnWrongType)
        {
            return this.GetValue(string.Empty, key).CastAsT<T>(defaultValue, separators, errorOnWrongType);
        }

        /// <summary>
        /// checks for the value in the config based on section
        /// </summary>
        /// <param name="section">e.g. [WINDOWS SETTINGS]</param>
        /// <param name="key">the key</param>
        /// <returns>the value</returns>
        private string GetValue(string section, string key)
        {

            using (var fs = new FileStream(this._iniPath, FileMode.Open))
            {
                using (var sr = new StreamReader(fs, this._encoding))
                {
                    var thisSection = string.Empty;
                    while (!sr.EndOfStream)
                    {
                        var thisLine = sr.ReadLine();
                        if (thisLine != null)
                        {
                            if (thisLine.Trim().StartsWith("[") && thisLine.Trim().EndsWith("]"))
                            {
                                // this line is a section marker
                                thisSection = thisLine.Trim().TrimStart('[').TrimEnd(']');
                            }

                            if (thisSection == section || string.IsNullOrEmpty(section))
                            {
                                if (this._validDelimiters.Any(aa => thisLine.StartsWith($"{key}{aa}", StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    return thisLine.Substring(key.Length + 1);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
