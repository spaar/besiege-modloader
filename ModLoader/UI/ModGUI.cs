using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permissions.
  // All credit goes to VapidLinus.
  // It was adapated for this project.
  public class ModGUI : SingleInstance<ModGUI>
  {
    public override string Name { get { return "spaar's Mod Loader: GUI Util"; } }

    /// <summary>
    /// The GUISkin used by the modloader and all mod's default config menus.
    /// This value can be used by mods if they want to look consistent to the modloader and other mods.
    /// The value can also be changed to change the skin for all that uses this.
    /// </summary>
    public static GUISkin Skin
    {
      get { return Instance._skin; }
      set { Instance._skin = value; }
    }
    private GUISkin _skin;

    void Awake()
    {
      ModLoader.Internal.ModLoader.MakeModule(this);

      // Not calling RebuildSkin(), because it will
      // be called when Elements is rebuilt.
    }

    void OnGUI()
    {
      if (!Elements.IsInitialized)
      {
        Elements.RebuildElements();
      }
    }

    /// <summary>
    /// Rebuilds the skin to match the GUIStyles in Elements.
    /// </summary>
    public void RebuildSkin()
    {
      Skin = ScriptableObject.CreateInstance<GUISkin>();
      Skin.window = Elements.Windows.Default;
      Skin.label = Elements.Labels.Default;
      Skin.button = Elements.Buttons.Default;
      Skin.textField = Skin.textArea = Elements.InputFields.Default;
      Skin.horizontalScrollbar = Elements.Scrollview.Horizontal;
      Skin.verticalScrollbar = Elements.Scrollview.Vertical;
      Skin.verticalScrollbarThumb = Elements.Scrollview.ThumbVertical;
      Skin.horizontalScrollbar = Elements.Scrollview.Horizontal;
      Skin.horizontalScrollbarThumb = Elements.Scrollview.ThumbHorizontal;
      Skin.scrollView = Elements.Windows.ClearDark;
      Skin.horizontalSlider = Elements.Sliders.Horizontal;
      Skin.horizontalSliderThumb = Elements.Sliders.ThumbHorizontal;
      Skin.verticalSlider = Elements.Sliders.Vertical;
      Skin.verticalSliderThumb = Elements.Sliders.ThumbVertical;
      Skin.toggle = Elements.Toggle.Default;
    }
  }
}