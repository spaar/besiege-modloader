using UnityEngine;

namespace spaar
{
    internal class KeySettings : MonoBehaviour
    {
        private readonly bool[] buttons = new bool[3];
        private bool Key1Pressed;
        private bool Key2Pressed = true;
        public KeyCode[] keyCode = new KeyCode[6];
        private Rect textRect;
        private bool visible;
        private bool waitingForKey1;
        private bool waitingForKey2;
        private bool waitingForKey3;
        private Rect windowRect;

        public KeySettings()
        {
            ConfigManager.LoadOrCreateConfig();
            keyCode[0] = ConfigManager.GetKey("ConsoleK").Modifier;
            keyCode[1] = ConfigManager.GetKey("ConsoleK").Trigger;
            keyCode[2] = ConfigManager.GetKey("OEK").Modifier;
            keyCode[3] = ConfigManager.GetKey("OEK").Trigger;
            keyCode[4] = ConfigManager.GetKey("SettingsK").Modifier;
            keyCode[5] = ConfigManager.GetKey("SettingsK").Trigger;
        }

        private void OnEnable()
        {
            windowRect = new Rect(Screen.width - 210.0f, Screen.height - 205.0f, 210.0f, 205.0f);
            textRect = new Rect(5.0f, 20.0f, 200.0f, 30.0f);
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(-1002, windowRect, OnWindow, "Keyboard Shortcuts Settings");
            }
        }

        private void OnWindow(int windowID)
        {
            buttons[0] = GUI.Button(new Rect(5.0f, 50.0f, 200.0f, 50.0f), "Console\n" + ConfigManager.GetKey("ConsoleK").Modifier + " + " + ConfigManager.GetKey("ConsoleK").Trigger);
            buttons[1] = GUI.Button(new Rect(5.0f, 150.0f, 200.0f, 50.0f), "Object Explorer\n" + ConfigManager.GetKey("OEK").Modifier + " + " + ConfigManager.GetKey("OEK").Trigger);
            buttons[2] = GUI.Button(new Rect(5.0f, 200.0f, 200.0f, 50.0f), "Settings\n" + ConfigManager.GetKey("SettingsK").Modifier + " + " + ConfigManager.GetKey("SettingsK").Trigger)
            ;
            
            if (buttons[0])
            {
                waitingForKey1 = true;
            }
            if (buttons[1])
            {
                waitingForKey2 = true;
            }
            if (buttons[2])
            {
                waitingForKey3 = true;
            }

            if (waitingForKey1)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[0] = e.keyCode;
                    }
                }
                if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.2");
                    if (e.isKey && e.keyCode != keyCode[0])
                    {
                        Key2Pressed = true;
                        waitingForKey1 = false;
                        buttons[0] = false;
                        keyCode[1] = e.keyCode;
                    }
                }
            }
            if (waitingForKey2)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[2] = e.keyCode;
                    }
                }
                if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.2");
                    if (e.isKey && e.keyCode != keyCode[2])
                    {
                        Key2Pressed = true;
                        waitingForKey2 = false;
                        buttons[1] = false;
                        keyCode[3] = e.keyCode;
                    }
                }
            }
            if (waitingForKey3)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[4] = e.keyCode;
                    }
                }
                if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key No.2");
                    if (e.isKey && e.keyCode != keyCode[4])
                    {
                        Key2Pressed = true;
                        waitingForKey3 = false;
                        buttons[2] = false;
                        keyCode[5] = e.keyCode;
                    }
                }
            }
            if (Key1Pressed && Key2Pressed)
            {
                Key1Pressed = false;
                Key2Pressed = true;
                ConfigManager.SaveChangedKeys();
            }
            GUI.DragWindow();
        }

        private void Update()
        {
            if (Input.GetKey(ConfigManager.GetKey("SettingsK").Modifier) && Input.GetKeyDown(ConfigManager.GetKey("SettingsK").Trigger))
            {
                visible = !visible;
            }
        }

        private void OnApplicationQuit()
        {
            ConfigManager.SaveChangedKeys();
        }
    }
}