using UnityEngine;

namespace spaar
{
    internal class KeySettings : MonoBehaviour
    {
        private readonly bool[] buttons = new bool[3];
        private bool Key1Pressed;
        private bool Key2Pressed = true;
        public KeyCode[] keyCode = new KeyCode[6];
        // private bool keysLoaded;
        // private Vector2 scrollPosition;
        private Rect textRect;
        private bool visible;
        private bool waitingForKey1;
        private bool waitingForKey2;
        private bool waitingForKey3;
        private Rect windowRect;

        public KeySettings()
        {
            KeyManager.LoadKeys();
            keyCode[0] = KeyGetter.getKey("ConsoleK").Modifier;
            keyCode[1] = KeyGetter.getKey("ConsoleK").Trigger;
            keyCode[2] = KeyGetter.getKey("OEK").Modifier;
            keyCode[3] = KeyGetter.getKey("OEK").Trigger;
            keyCode[4] = KeyGetter.getKey("SettingsK").Modifier;
            keyCode[5] = KeyGetter.getKey("SettingsK").Trigger;
        }

        private void OnEnable()
        {
            windowRect = new Rect(Screen.width - 210.0f, Screen.height - 305.0f, 210.0f, 305.0f);
            textRect = new Rect(5.0f, 20.0f, 200.0f, 100.0f);
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(-1002, windowRect, OnWindow, "KeySettings");
            }
        }

        private void OnWindow(int windowID)
        {
            buttons[0] = GUI.Button(new Rect(5.0f, 150.0f, 200.0f, 50.0f), "Console Keys");
            buttons[1] = GUI.Button(new Rect(5.0f, 200.0f, 200.0f, 50.0f), "Object Explorer Keys");
            buttons[2] = GUI.Button(new Rect(5.0f, 250.0f, 200.0f, 50.0f), "KeySettings Keys");
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
                KeyGetter.saveKeys();
            }
            GUI.DragWindow();
        }

        private void Update()
        {
            if (Input.GetKey(KeyGetter.getKey("SettingsK").Modifier) && Input.GetKeyDown(KeyGetter.getKey("SettingsK").Trigger))
            {
                visible = !visible;
            }
        }

        private void OnApplicationQuit()
        {
            KeyManager.saveKeys();
        }
    }
}