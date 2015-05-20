using UnityEngine;

namespace spaar
{
    internal class KeySettings : MonoBehaviour
    {
        private bool consoleButton, objExpButton, settingsButton;
        private bool Key1Pressed;
        private bool Key2Pressed = true;
        public string consoleK1_value, consoleK2_value, objExpK1_value, objExpK2_value, settingsK1_value, settingsK2_value;
        private bool visible;
        private bool waitingForConsoleKey, waitingForObjExpKey, waitingForSettingsKey;
        private Rect windowRect;
        private Rect textRect;

        public KeySettings()
        {
            consoleK1_value = "LeftControl";
            consoleK2_value = "K";
            objExpK1_value = "LeftControl";
            objExpK2_value = "O";
            settingsK1_value = "LeftControl";
            settingsK2_value = "L";
        }

        private void OnEnable()
        {
            //Modular windowRect to fit to screen
            //Don't use old one
            //windowRect = new Rect(700f, 300f, 210f, 400f);
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
            consoleButton = GUI.Button(new Rect(5.0f, 50.0f, 200.0f, 50.0f), "Console\n(" + Keys.getKey("Console").Modifier + " + " + Keys.getKey("Console").Trigger + ")");
            objExpButton = GUI.Button(new Rect(5.0f, 100.0f, 200.0f, 50.0f), "Object Explorer\n(" + Keys.getKey("ObjectExplorer").Modifier + " + " + Keys.getKey("ObjectExplorer").Trigger + ")");
            settingsButton = GUI.Button(new Rect(5.0f, 150.0f, 200.0f, 50.0f), "Settings\n(" + Keys.getKey("Settings").Modifier + " + " + Keys.getKey("Settings").Trigger + ")");
            if (consoleButton)
            {
                waitingForConsoleKey = true;
            }
            if (objExpButton)
            {
                waitingForObjExpKey = true;
            }
            if (settingsButton)
            {
                waitingForSettingsKey = true;
            }

            if (waitingForConsoleKey)
            {
                var keyEvent = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (keyEvent.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        consoleK1_value = keyEvent.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (keyEvent.isKey && keyEvent.keyCode.ToString() != consoleK1_value)
                    {
                        Key2Pressed = true;
                        waitingForConsoleKey = false;
                        consoleK2_value = keyEvent.keyCode.ToString();
                    }
                }
            }
            if (waitingForObjExpKey)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        objExpK1_value = e.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (e.isKey && e.keyCode.ToString() != objExpK1_value)
                    {
                        Key2Pressed = true;
                        waitingForObjExpKey = false;
                        objExpK2_value = e.keyCode.ToString();
                    }
                }
            }
            if (waitingForSettingsKey)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        settingsK1_value = e.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (e.isKey && e.keyCode.ToString() != settingsK1_value)
                    {
                        Key2Pressed = true;
                        waitingForSettingsKey = false;
                        settingsK2_value = e.keyCode.ToString();
                    }
                }
            }
            if (Key1Pressed && Key2Pressed)
            {
                Key1Pressed = false;
                Key2Pressed = false;

                Configuration configuration = ModLoader.Configuration;
                configuration.consoleK1 = consoleK1_value;
                configuration.consoleK2 = consoleK2_value;
                configuration.objExpK1 = objExpK1_value;
                configuration.objExpK2 = objExpK2_value;
                configuration.settingsK1 = settingsK1_value;
                configuration.settingsK2 = settingsK2_value;
                Configuration.SaveConfig(Configuration.CONFIG_FILE_NAME, configuration);
                Keys.LoadKeys();
            }
            GUI.DragWindow();
        }

        private void Update()
        {
            if (Input.GetKey(Keys.getKey("Settings").Modifier) && Input.GetKeyDown(Keys.getKey("Settings").Trigger))
            {
                visible = !visible;
            }
        }
    }
}
