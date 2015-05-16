using System.IO;

/*
Example of usage:

Configuration myconfig = new Configuration();
            myconfig.ConsoleK1 = "LeftControl";
            myconfig.ConsoleK2 = "K";
            myconfig.OEK1 = "LeftControl";
            myconfig.OEK2 = "O";
            myconfig.SettingsK1 = "LeftControl";
            myconfig.SettingsK2 = "L";
            Configuration.SaveConfig("123.txt", myconfig);

            Configuration loadconfig = new Configuration();
            loadconfig = Configuration.LoadConfig("123.txt");
            Debug.Log("\n" + loadconfig.ConsoleK1 + "\n" + loadconfig.ConsoleK2 + "\n" + loadconfig.OEK1 + "\n" + loadconfig.OEK2 + "\n" + loadconfig.SettingsK1 + "\n" + loadconfig.SettingsK2);
*/

namespace spaar
{
    public class Configuration
    {
        public string ConsoleK1;
        public string ConsoleK2;
        public string OEK1;
        public string OEK2;
        public string SettingsK1;
        public string SettingsK2;

        public static void SaveConfig(string fileName, Configuration c)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(fileName);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }
        public static Configuration LoadConfig(string fileName)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
            StreamReader reader = File.OpenText(fileName);
            Configuration c = (Configuration)xs.Deserialize(reader);
            reader.Close();
            return c;
        }
    }
}
