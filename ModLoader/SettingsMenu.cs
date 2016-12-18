using System;
using System.Collections.Generic;
using spaar.ModLoader.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spaar.ModLoader
{
    /// <summary>
    ///     Callback delegate for a toggle setting.
    /// </summary>
    /// <param name="active">Whether the toggle is active</param>
    public delegate void SettingsToggle(bool active);

    /// <summary>
    /// </summary>
    public class SettingsButton
    {
        private bool _created;
        private int _fontSize;

        private string _text;
        private bool _value;

        internal Action OnDestroy;
        internal Action<int> OnFontSizeChange;
        internal Action<string> OnTextChange;

        /// <summary>
        ///     Called on Value change.
        /// </summary>
        public SettingsToggle OnToggle;

        public string Text
        {
            get { return _text; }
            set
            {
                OnTextChange?.Invoke(value);
                _text = value;
            }
        }

        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                OnFontSizeChange?.Invoke(value);
                _fontSize = value;
            }
        }

        public bool Value
        {
            get { return _value; }
            set
            {
                OnToggle?.Invoke(value);
                _value = value;
            }
        }

        public void Create()
        {
            if (_created) return;
            SettingsMenu.RegisterSettingsButton(this);
            _created = true;
        }

        public void Destroy()
        {
            OnDestroy?.Invoke();
            OnToggle = null;
            OnDestroy = null;
            OnFontSizeChange = null;
            OnTextChange = null;
            _created = false;
        }
    }

    /// <summary>
    ///     SettingsMenu contains methods for adding a setting (toggle button) to the
    ///     settings drop-down of Besiege.
    /// </summary>
    public class SettingsMenu : SingleInstance<SettingsMenu>
    {
        private static int _numRegistered;

        private static readonly List<SettingsButton> ToAdd = new List<SettingsButton>();

        private static Transform _modSection;

        public override string Name => "spaar's Mod Loader: Settings Utility";

        /// <summary>
        ///     Registers a new toggle button. It will be placed below all others
        ///     that are currently registered.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="cb">Callback to call when the button is clicked</param>
        /// <param name="defaultValue">Starting state of the toggle</param>
        /// <param name="fontSize">Font size of the text on the button</param>
        [Obsolete(
            "RegisterSettingsButton is deprecated. Please initialize SettingsButton class instead and call it's Create method."
        )]
        public static void RegisterSettingsButton(string text, SettingsToggle cb,
            bool defaultValue = false, int fontSize = 0)
        {
            var button = new SettingsButton
            {
                Text = text,
                OnToggle = cb,
                Value = defaultValue,
                FontSize = fontSize
            };
            ToAdd.Add(button);

            if (Game.AddPiece != null)
                RegisterSettingsButtonInternal(button);
        }

        internal static void RegisterSettingsButton(SettingsButton button)
        {
            ToAdd.Add(button);

            if (Game.AddPiece != null)
                RegisterSettingsButtonInternal(button);
        }

        private void Start()
        {
            Internal.ModLoader.MakeModule(this);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Game.AddPiece != null)
            {
                _numRegistered = 0;
                foreach (var button in ToAdd)
                    RegisterSettingsButtonInternal(button);
            }
        }

        private static void RegisterSettingsButtonInternal(SettingsButton button)
        {
            var settingsObjects = GameObject.Find("Settings").transform
                .FindChild("SettingsObjects");
            var bottomDefaultSetting = settingsObjects.FindChild("GOD/INFINITE AMMO");
            var settingSize = new Vector3(0.748f, 0.375f);

            if (_modSection == null)
            {
                // Create a MODS section
                var settings = settingsObjects.FindChild("SETTINGS");

                var modsPos = settings.position;
                modsPos.y = bottomDefaultSetting.position.y - 1.2f;
                modsPos.z = 7.0f; // Prevent blocks GUI from being over the toggles

                _modSection = (Transform) Instantiate(settings, modsPos,
                    settings.rotation);
                _modSection.parent = settingsObjects;
                _modSection.name = "MOD SETTINGS";

                foreach (Transform child in _modSection)
                    if (child.name == "GENERAL")
                    {
                        child.GetComponent<TextMesh>().text = "M O D S";
                        child.name = "Title";
                    }
                    else
                    {
                        Destroy(child.gameObject);
                    }

                // Adjust background to include mods section title
                var bg = settingsObjects.FindChild("BG");
                var bgScale = bg.localScale;
                bgScale.y += 2.85f;
                bg.localScale = bgScale;
                // Also need to remove background of infinite ammo as long as it's
                // the only thing in its row.
                Destroy(settingsObjects.FindChild("GOD/INFINITE AMMO/BG (1)").gameObject);

                bg.gameObject.AddComponent<BoxCollider>();
                var scrollCollider = new GameObject("Scrolling").transform;
                scrollCollider.parent = settingsObjects;
                scrollCollider.rotation = bg.rotation;
                scrollCollider.localScale = bg.localScale;
                scrollCollider.gameObject.layer = bg.gameObject.layer;
                var pos = bg.position;
                pos.z = 15.0f; // Put collider behind all settings items
                scrollCollider.position = pos;
                scrollCollider.gameObject.AddComponent<ScrollSettingsMenu>()
                    .settingsObjects = settingsObjects;
            }
            var settingPos = bottomDefaultSetting.position;

            settingPos.x += _numRegistered % 2 * settingSize.x;
            settingPos.y -= 1.25f + _numRegistered / 2 * settingSize.y;
            settingPos.z = 7.0f; // Prevent blocks GUI from being over any toggles

            var fxaa = settingsObjects.FindChild("SETTINGS/FXAA");

            var newSetting = (Transform) Instantiate(fxaa, settingPos, fxaa.rotation);
            newSetting.parent = _modSection;

            if (_numRegistered % 2 == 0)
            {
                // Expand background to include new toggle
                var background = settingsObjects.FindChild("BG");
                var backgroundScale = background.localScale;
                backgroundScale.y += settingSize.y * 2;
                background.localScale = backgroundScale;

                // Expand the scrolling object to the same size
                var scrolling = settingsObjects.FindChild("Scrolling");
                scrolling.localScale = backgroundScale;
                scrolling.GetComponent<ScrollSettingsMenu>().CalcBounds();

                // Check whether the new element row is outside of the screen
                // and enable scrolling if it is
                var lowestPoint = newSetting.position - settingSize;
                var cam = GameObject.Find("HUD Cam").GetComponent<Camera>();
                if (cam.WorldToViewportPoint(lowestPoint).y < 0.0f)
                    scrolling.GetComponent<ScrollSettingsMenu>().scrollingEnabled = true;
            }

            var newSettingButton = newSetting.FindChild("AA BUTTON");
            var newSettingText = newSetting.FindChild("AA text");
            newSetting.gameObject.name = button.Text;
            newSettingButton.gameObject.name = button.Text + " button";
            newSettingText.gameObject.name = button.Text + " text";
            var textMesh = newSettingText.gameObject.GetComponent<TextMesh>();
            textMesh.text = button.Text;
            textMesh.fontSize = button.FontSize;

            var settingsComponent
                = newSettingButton.gameObject.AddComponent<SettingsComponent>();

            var fxaaToggle = newSettingButton.gameObject.GetComponent<ToggleAA>();
            settingsComponent.darkMaterial = fxaaToggle.darkMaterial;
            settingsComponent.redMaterial = fxaaToggle.redMaterial;
            Destroy(fxaaToggle);

            settingsComponent.SetCallback(button.OnToggle);
            settingsComponent.SetOn(button.Value);

            // Change material on button OnToggle
            button.OnToggle += value => settingsComponent.SetOn(value);

            // Change textMesh on Text property change
            button.OnTextChange += value => textMesh.text = value;

            // Change font size on FontSize property change
            button.OnFontSizeChange += value => textMesh.fontSize = value;

            // Set button OnDestroy method
            button.OnDestroy += () =>
            {
                Destroy(newSetting.gameObject);
                _numRegistered--;
            };

            _numRegistered++;
        }
    }
}