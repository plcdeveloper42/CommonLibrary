using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/*
Copyright © 2023 plcdeveloper42@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the “Software”), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace PlcDeveloper42.Common
{
    /// <summary>
    /// Simple persistence class for storing values and retrieving it at the next program start
    /// </summary>
    public class Persistence
    {
        #region singleton
        private static Persistence _instance;

        public static Persistence Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Persistence();
                }
                return _instance;
            }
        }
        #endregion

        #region public properties
        public string AppName { get; set; } = "";
        public string FileName { get; set; } = "Persistence.json";
        #endregion

        #region set and get values
        /// <summary>
        /// Returns a string value by a given key
        /// </summary>
        /// <param name="key">The unique key under which the value was stored</param>
        /// <returns>The string value</returns>
        public string GetValue(string key)
        {
            var dict = ReadDictionary();
            if (dict.ContainsKey(key))
                return dict[key];
            return "";
        }

        /// <summary>
        /// Returns an integer value by a given key
        /// </summary>
        /// <param name="key">The unique key under which the value was stored</param>
        /// <returns>The integer value</returns>
        public int GetIntValue(string key)
        {
            var s = GetValue(key);
            if (int.TryParse(s, out var val))
                return val;
            return 0;
        }

        /// <summary>
        /// Stores a string value by a given key
        /// </summary>
        /// <param name="key">The unique key under which the value was stored</param>
        /// <returns></returns>
        public void SetValue(string key, string value)
        {
            var dict = ReadDictionary();
            dict[key] = value;
            WriteDictionary(dict);
        }

        /// <summary>
        /// Stores an integer value by a given key
        /// </summary>
        /// <param name="key">The unique key under which the value was stored</param>
        /// <returns></returns>
        public void SetIntValue(string key, int value)
        {
            SetValue(key, value.ToString());
        }
        #endregion

        #region helper
        private string GetFilePath()
        {
            var assemblyName = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Name;

            if (AppName == "")
                AppName = assemblyName;

            string p = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(p, AppName, FileName);
        }

        private string GetFileText()
        {
            var filePath = GetFilePath();
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(filePath))
                File.WriteAllText(filePath, "");
            return File.ReadAllText(filePath);
        }

        private Dictionary<string, string> ReadDictionary()
        {
            var text = GetFileText();
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            if (dict == null)
                return new Dictionary<string, string>();

            return dict;
        }

        private void WriteDictionary(Dictionary<string, string> dict)
        {
            var filePath = GetFilePath();
            var text = JsonConvert.SerializeObject(dict);
            File.WriteAllText(filePath, text);
        }
        #endregion
    }
}
