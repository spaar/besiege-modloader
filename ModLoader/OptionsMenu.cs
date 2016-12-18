using System;
using System.Collections.Generic;
using spaar.ModLoader.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spaar.ModLoader
{
    /// <summary>
    ///     Callback delegate for an option toggle.
    /// </summary>
    /// <param name="active">Whether the toggle is active</param>
    public delegate void OptionsToggle(bool active);

    /// <summary>
    /// </summary>
    public class OptionsButton
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
        public OptionsToggle OnToggle;

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
            OptionsMenu.RegisterOptionsToggle(this);
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
    ///     OptionsMenu contains methods for adding an option (toggle button) to the
    ///     options menu in the main menu of Besiege.
    /// </summary>
    public class OptionsMenu : SingleInstance<OptionsMenu>
    {
        private static int _numRegistered;

        private static readonly List<OptionsButton> ToAdd = new List<OptionsButton>();

        private static Transform _optionsList;
        private static Transform _windowedText;
        private static Transform _windowedTickBox;
        public override string Name { get; } = "spaar's Mod Loader: Options Utility";

        /// <summary>
        ///     Registers a new toggle button. It will be placed below all others that
        ///     are currently registered.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="cb">Callback to call when the button is clicked</param>
        /// <param name="defaultValue">Starting state of the toggle</param>
        /// <param name="fontSize">Font size of the text on the button</param>
        [Obsolete(
            "RegisterOptionsButton is deprecated. Please initialize OptionsButton class instead and call it's Create method."
        )]
        public static void RegisterOptionsToggle(string text, OptionsToggle cb,
            bool defaultValue = false, int fontSize = 0)
        {
            var button = new OptionsButton
            {
                Text = text,
                OnToggle = cb,
                Value = defaultValue,
                FontSize = fontSize
            };
            ToAdd.Add(button);

            if (SceneManager.GetActiveScene().buildIndex == 1) // Main Menu
                RegisterOptionsToggleInternal(button);
        }

        internal static void RegisterOptionsToggle(OptionsButton button)
        {
            ToAdd.Add(button);

            if (SceneManager.GetActiveScene().buildIndex == 1) // Main Menu
                RegisterOptionsToggleInternal(button);
        }

        private void Start()
        {
            Internal.ModLoader.MakeModule(this);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1) // Main Menu
            {
                _numRegistered = 0;
                foreach (var button in ToAdd)
                    RegisterOptionsToggleInternal(button);
            }
        }

        private static void RegisterOptionsToggleInternal(OptionsButton button)
        {
            if (_optionsList == null)
            {
                GameObject.Find("O P T I O N S")
                    .GetComponent<EnableObjOnClick>().OnMouseUp();
                _optionsList = GameObject.Find("OPTIONS LIST").transform;
            }

            if (_numRegistered == 0)
            {
                // Move Windowed setting up to make room
                _windowedText = _optionsList.FindChild("WINDOWED");
                _windowedText.Translate(0f, 1.3f, 0f);
                _windowedTickBox = _optionsList.FindChild("WINDOWED TICK BOX");
                _windowedTickBox.Translate(0f, 1.3f, 0f);
            }

            var textPos = _windowedText.position
                          + Vector3.down * 0.45f * (_numRegistered + 1);
            var textScale = _windowedText.localScale;
            var boxPos = _windowedTickBox.position
                         + Vector3.down * 0.45f * (_numRegistered + 1);
            var boxScale = _windowedTickBox.localScale;

            var newText = (Transform) Instantiate(_windowedText, textPos,
                Quaternion.identity);
            var newBox = (Transform) Instantiate(_windowedTickBox, boxPos,
                Quaternion.identity);

            newText.parent = _optionsList;
            newBox.parent = _optionsList;

            newText.name = button.Text + " (Modded)";
            newBox.name = button.Text + "TickBox (Modded)";

            newText.localScale = textScale;
            newBox.localScale = boxScale;

            newText.GetComponent<TextMesh>().text = button.Text;
            if (button.FontSize != 0)
                newText.GetComponent<TextMesh>().fontSize = button.FontSize;

            var fullscreenController = newBox.GetComponent<FullScreenController>();
            var optionsComponent = newBox.gameObject.AddComponent<OptionsComponent>();

            optionsComponent.darkMaterial = fullscreenController.darkMaterial;
            optionsComponent.redMaterial = fullscreenController.redMaterial;
            Destroy(fullscreenController);

            optionsComponent.SetCallback(button.OnToggle);
            optionsComponent.SetOn(button.Value);

            // Change material on button OnToggle
            button.OnToggle += value => optionsComponent.SetOn(value);

            // Change textMesh on Text property change
            button.OnTextChange += value => newText.GetComponent<TextMesh>().text = value;

            // Change font size on FontSize property change
            button.OnFontSizeChange += value => newText.GetComponent<TextMesh>().fontSize = value;

            // Set button OnDestroy method
            button.OnDestroy += () =>
            {
                Destroy(newText.gameObject);
                Destroy(newBox.gameObject);
                _numRegistered--;
            };

            _numRegistered++;

            _optionsList.FindChild("QUIT WINDOW ICON")
                .GetComponent<DisableObjOnClick>().OnMouseUp();
        }
    }
}