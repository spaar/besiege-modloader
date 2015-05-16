using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace spaar
{
    internal class FilePropertyStream
    {
        private string filename;
        private Dictionary<string, string> list;

        public FilePropertyStream(string file)
        {
            reload(file);
        }

        public string get(string field, string defValue)
        {
            return (get(field) == null) ? (defValue) : (get(field));
        }

        public string get(string field)
        {
            return (list.ContainsKey(field)) ? (list[field]) : (null);
        }

        public void set(string field, object value)
        {
            if (!list.ContainsKey(field))
                list.Add(field, value.ToString());
            else
                list[field] = value.ToString();
        }

        public void Save()
        {
            Save(filename);
        }

        public void Save(string filename)
        {
            this.filename = filename;

            if (!File.Exists(filename))
                File.Create(filename);

            var file = new StreamWriter(filename);

            foreach (var prop in list.Keys.ToArray())
            {
                file.WriteLine(prop + "=" + list[prop]);
            }
            file.Close();
        }

        public void reload()
        {
            reload(filename);
        }

        public void reload(string filename)
        {
            this.filename = filename;
            list = new Dictionary<string, string>();

            if (File.Exists(filename))
                loadFromFile(filename);
            else
                File.Create(filename);
        }

        private void loadFromFile(string file)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    var index = line.IndexOf('=');
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        //ignore dublicates
                        list.Add(key, value);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void OnApplicationQuit()
        {
            Save();
        }
    }
}
