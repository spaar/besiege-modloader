using System;
using System.IO;
using UnityEngine;

namespace spaar
{
    internal class Keys : MonoBehaviour
    {
        private static Key ConsoleK;
        private static Key OEK;
        private static Key SettingsK;
        private static KeyCode[] keyNames;
        private static string[] fpsKeyNames;
        private static FilePropertyStream fps;

        public static void loadKeys()
        {
            ConsoleK = new Key();
            OEK = new Key();
            SettingsK = new Key();
            if (!Directory.Exists(Application.dataPath + "/Mods/Config"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Mods/Config");
                StreamWriter sw = File.CreateText(Application.dataPath + "/Mods/Config/Keys.txt");
                sw.WriteLine("ConsoleK1=LeftControl");
                sw.WriteLine("ConsoleK2=K");
                sw.WriteLine("OEK1=LeftControl");
                sw.WriteLine("OEK2=O");
                sw.WriteLine("SettingsK1=LeftControl");
                sw.WriteLine("SettingsK2=L");
                sw.Close();
            }

            fps = new FilePropertyStream(string.Concat(Application.dataPath, "/Mods/Config/", "Keys.txt"));

            fpsKeyNames = new string[6];

            ConsoleK.addKey(true, fps.get("ConsoleK1"));
            ConsoleK.addKey(false, fps.get("ConsoleK2"));
            OEK.addKey(true, fps.get("OEK1"));
            OEK.addKey(false, fps.get("OEK2"));
            SettingsK.addKey(true, fps.get("SettingsK1"));
            SettingsK.addKey(false, fps.get("SettingsK2"));
            Debug.Log("Assigned");
        }

        public static void LoadKeysAssigned()
        {
            var i = 0;
            keyNames = GameObject.Find("MODLOADERLORD").GetComponent<KeySettings>().keyCode;
            foreach (var keyCodes in keyNames)
            {
                if (keyCodes != KeyCode.Alpha0)
                {
                    fpsKeyNames[i] = Enum.GetName(typeof (KeyCode), keyCodes);
                    switch (i)
                    {
                        case 0:
                        {
                            setKey("ConsoleK1", fpsKeyNames[i]);
                            break;
                        }
                        case 1:
                        {
                            setKey("ConsoleK2", fpsKeyNames[i]);
                            break;
                        }
                        case 2:
                        {
                            setKey("OEK1", fpsKeyNames[i]);
                            break;
                        }
                        case 3:
                        {
                            setKey("OEK2", fpsKeyNames[i]);
                            break;
                        }
                        case 4:
                        {
                            setKey("SettingsK1", fpsKeyNames[i]);
                            break;
                        }
                        case 5:
                        {
                            setKey("SettingsK2", fpsKeyNames[i]);
                            break;
                        }
                    }
                }
                i++;
            }
            OnApplicationQuit();
        }

        public static void setKey(string KeyName, string Input)
        {
            switch (KeyName)
            {
                case "ConsoleK1":
                {
                    ConsoleK.addKey(true, Input);
                    break;
                }
                case "ConsoleK2":
                {
                    ConsoleK.addKey(false, Input);
                    break;
                }
                case "OEK1":
                {
                    OEK.addKey(true, Input);
                    break;
                }
                case "OEK2":
                {
                    OEK.addKey(false, Input);
                    break;
                }
                case "SettingsK1":
                {
                    SettingsK.addKey(true, Input);
                    break;
                }
                case "SettingsK2":
                {
                    SettingsK.addKey(false, Input);
                    break;
                }
            }
        }

        public static void OnApplicationQuit()
        {
            fps.set("ConsoleK1", ConsoleK.getKey(true));
            fps.set("ConsoleK2", ConsoleK.getKey(false));
            fps.set("OEK1", OEK.getKey(true));
            fps.set("OEK2", OEK.getKey(false));
            fps.set("SettingsK1", SettingsK.getKey(true));
            fps.set("SettingsK2", SettingsK.getKey(false));
            fps.Save();
        }

        public static KeyCode getKey(string keyName)
        {
            KeyCode key = KeyCode.Alpha0;
            switch (keyName)
            {
                case "ConsoleK1":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsoleK.getKey(true));
                    break;
                }
                case "ConsoleK2":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsoleK.getKey(false));
                    break;
                }
                case "OEK1":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), OEK.getKey(true));
                    break;
                }
                case "OEK2":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), OEK.getKey(false));
                    break;
                }
                case "SettingsK1":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), SettingsK.getKey(true));
                    break;
                }
                case "SettingsK2":
                {
                    key = (KeyCode)System.Enum.Parse(typeof(KeyCode), SettingsK.getKey(false));
                    break;
                }
            }
            return key;
        }
    }

    internal class Key
    {
        private readonly string[] Inputs = new string[2];

        public void addKey(bool First, string Input)
        {
            if (First)
            {
                Inputs[0] = Input;
            }
            else
            {
                Inputs[1] = Input;
            }
        }

        public string getKey(bool first)
        {
            if (first)
            {
                return Inputs[0];
            }
            return Inputs[1];
        }
    }
}
