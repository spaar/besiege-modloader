using System;
using UnityEngine;

namespace spaar
{
    internal class KeySettings : MonoBehaviour
    {
        private readonly bool[] buttons = new bool[3];
        private bool Key1Pressed;
        private bool Key2Pressed = true;
        public string[] keyCode = new string[6];
        private bool visible;
        private bool waitingForKey1;
        private bool waitingForKey2;
        private bool waitingForKey3;
        private Rect windowRect;
        private Rect textRect;

        public KeySettings()
        {
            keyCode[0] = "LeftControl";
            keyCode[1] = "K";
            keyCode[2] = "LeftControl";
            keyCode[3] = "O";
            keyCode[4] = "LeftControl";
            keyCode[5] = "L";
        }

        private void OnEnable()
        {
            //Modular windowRect to fit to screen
            //Don't use old one
            //windowRect = new Rect(700f, 300f, 210f, 400f);
            windowRect = new Rect(Screen.width - 210.0f, Screen.height - 305.0f, 210.0f, 305.0f);
            textRect = new Rect(5.0f, 20.0f, 200.0f, 100.0f);
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(-1002, windowRect, OnWindow, "Settings");
            }
        }

        private void OnWindow(int windowID)
        {
            buttons[0] = GUI.Button(new Rect(5.0f, 150.0f, 200.0f, 50.0f), "Console Keys");
            buttons[1] = GUI.Button(new Rect(5.0f, 200.0f, 200.0f, 50.0f), "Object Explorer Keys");
            buttons[2] = GUI.Button(new Rect(5.0f, 250.0f, 200.0f, 50.0f), "Settings Keys");
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
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[0] = e.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (e.isKey && e.keyCode.ToString() != keyCode[0])
                    {
                        Key2Pressed = true;
                        waitingForKey1 = false;
                        keyCode[1] = e.keyCode.ToString();
                    }
                }
            }
            if (waitingForKey2)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[2] = e.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (e.isKey && e.keyCode.ToString() != keyCode[2])
                    {
                        Key2Pressed = true;
                        waitingForKey2 = false;
                        keyCode[3] = e.keyCode.ToString();
                    }
                }
            }
            if (waitingForKey3)
            {
                var e = Event.current;
                if (!Key1Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 1");
                    if (e.isKey)
                    {
                        Key1Pressed = true;
                        Key2Pressed = false;
                        keyCode[4] = e.keyCode.ToString();
                    }
                }
                else if (!Key2Pressed)
                {
                    GUI.TextField(textRect, "Please Press Key 2");
                    if (e.isKey && e.keyCode.ToString() != keyCode[4])
                    {
                        Key2Pressed = true;
                        waitingForKey3 = false;
                        keyCode[5] = e.keyCode.ToString();
                    }
                }
            }
            if (Key1Pressed && Key2Pressed)
            {
                Key1Pressed = false;
                Key2Pressed = false;

                Configuration c = ModLoader.Configuration;
                c.ConsoleK1 = keyCode[0];
                c.ConsoleK2 = keyCode[1];
                c.OEK1 = keyCode[2];
                c.OEK2 = keyCode[3];
                c.SettingsK1 = keyCode[4];
                c.SettingsK2 = keyCode[5];
                Configuration.SaveConfig(Configuration.DefaultFileName, c);
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
