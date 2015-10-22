using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace spaar.ModLoader.UI
{
  /// <summary>
  /// Container for GUIStyle elements.
  /// </summary>
  public static class Elements
  {
    private static Settings _settings;
    public static Settings Settings { get { return _settings ?? (_settings = new Settings()); } }

    public static bool IsInitialized { get; private set; }

    public static Colors Colors { get; private set; }
    public static Windows Windows { get; private set; }
    public static Labels Labels { get; private set; }
    public static Buttons Buttons { get; private set; }
    public static Tools Tools { get; private set; }
    public static InputFields InputFields { get; private set; }
    public static Scrollview Scrollview { get; private set; }
    public static Sliders Sliders { get; private set; }
    public static Toggle Toggle { get; private set; }
    
    /// <summary>
    /// Rebuilds all elements to make them match the settings in Elements.Settings.
    /// <para>This will also call VGUI.Instance.RebuildSkin().</para>
    /// </summary>
    public static void RebuildElements()
    {
      IsInitialized = true;

      Colors = new Colors();
      Labels = new Labels();
      Windows = new Windows();
      Buttons = new Buttons();
      Tools = new Tools();
      InputFields = new InputFields();
      Scrollview = new Scrollview();
      Sliders = new Sliders();
      Toggle = new Toggle();

      ModGUI.Instance.RebuildSkin();
    }

    private static readonly Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
    internal static Texture2D LoadImage(string name)
    {
      if (loadedTextures.ContainsKey(name))
      {
        Texture2D image;
        loadedTextures.TryGetValue(name, out image);
        if (image != null) return image;
      }

      try
      {
        var bytes = File.ReadAllBytes(Application.dataPath + "/Mods/Resources/ModLoader/UI/" + name);
        var texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        loadedTextures.Add(name, texture);

        return texture;
      }
      catch (Exception e)
      {
        Debug.LogWarning("Failed to load: " + name);
        Debug.LogException(e);

        return null;
      }
    }
  }
}