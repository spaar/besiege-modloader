using System;
using UnityEngine;

namespace spaar
{
    internal class Key
    {
        public KeyCode Modifier { get; private set; }
        public KeyCode Trigger { get; private set; }

        public Key(string modifier, string key)
        {
            Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), modifier);
            Trigger = (KeyCode)Enum.Parse(typeof(KeyCode), key);
        }
    }
}
