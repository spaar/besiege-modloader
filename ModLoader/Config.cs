using System;
using UnityEngine;

namespace spaar
{
    /// <summary>
    /// Only used for serialization
    /// </summary>
    [Serializable]
    public class Config
    {
        public String CK1, CK2, OEK1, OEK2, SK1, SK2;
        public readonly String CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/Config.xml";
        //additional configurations can be added, like "public int windowSize"...
    }
    public class Key
    {
        public Key(String modifier, String key)
        {
            Modifier = (KeyCode)System.Enum.Parse(typeof (KeyCode), modifier);
            Trigger = (KeyCode) System.Enum.Parse(typeof (KeyCode), key);
        }

        public KeyCode Modifier { get; private set; }
        public KeyCode Trigger { get; private set; }
    }
}
