using System;
using System.Collections.Generic;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class InspectorPanel : MonoBehaviour
  {
    enum FieldType
    {
      Normal, VectorX, VectorY, VectorZ, ColorR, ColorG, ColorB, ColorA
    }

    private const string FIELD_EDIT_INPUT_NAME = "field_edit_input";

    private MemberValue activeMember;
    private FieldType activeMemberFieldType = FieldType.Normal;
    private object activeMemberNewValue;

    private bool layerTextInputActive;
    private string layerTextInputNewValue;

    private GameObject _selectedGameObject;
    // null indicates no object selected
    public GameObject SelectedGameObject
    {
      get
      {
        return _selectedGameObject;
      }
      set
      {
        _selectedGameObject = value;
        entries.Clear();

        if (value != null)
        {
          foreach (var component in value.GetComponents<Component>())
          {
            entries.Add(new ComponentEntry(component));
          }
        }
      }
    }

    public bool IsGameObjectSelected
    {
      get { return SelectedGameObject != null; }
    }

    private Dictionary<string, bool> filter = new Dictionary<string, bool>()
    {
      { "Instance", true },
      { "Static", false },
      { "Public", true },
      { "NonPublic", false },
      { "Inherited", false },
      { "Has Setter", true }, // TODO: reconsider default
    };

    private readonly HashSet<ComponentEntry> entries = new HashSet<ComponentEntry>();
    private Vector2 inspectorScroll = Vector2.zero;

    void OnGUI()
    {
      if (activeMember != null && Event.current.keyCode == KeyCode.Return)
      {
        object @object = activeMember.GetValue();

        if (@object is string || @object is bool)
        {
          activeMember.SetValue(activeMemberNewValue);
        }
        else if (@object is int)
        {
          int i;
          if (activeMemberNewValue != null
            && int.TryParse(activeMemberNewValue.ToString(), out i))
          {
            activeMember.SetValue(i);
          }
        }
        else if (@object is Vector3)
        {
          var vector3 = (Vector3)activeMember.GetValue();
          float v;
          if (activeMemberNewValue != null
            && float.TryParse(activeMemberNewValue.ToString(), out v))
          {
            switch (activeMemberFieldType)
            {
              case FieldType.VectorX: vector3.x = v; break;
              case FieldType.VectorY: vector3.y = v; break;
              case FieldType.VectorZ: vector3.z = v; break;
            }
          }
          activeMember.SetValue(vector3);
        }
        else if (@object is Color)
        {
          var color = (Color)activeMember.GetValue();
          float v;
          if (activeMemberNewValue != null
            && float.TryParse(activeMemberNewValue.ToString(), out v))
          {
            switch (activeMemberFieldType)
            {
              case FieldType.ColorR: color.r = v; break;
              case FieldType.ColorG: color.g = v; break;
              case FieldType.ColorB: color.b = v; break;
              case FieldType.ColorA: color.a = v; break;
            }
          }
          activeMember.SetValue(color);
        }
        else if (@object is Quaternion)
        {
          var quat = (Quaternion)activeMember.GetValue();
          float v;
          if (activeMemberNewValue != null
            && float.TryParse(activeMemberNewValue.ToString(), out v))
          {
            switch (activeMemberFieldType)
            {
              case FieldType.ColorR: quat.x = v; break;
              case FieldType.ColorG: quat.y = v; break;
              case FieldType.ColorB: quat.z = v; break;
              case FieldType.ColorA: quat.w = v; break;
            }
          }
          activeMember.SetValue(quat);
        }
        else if (@object is float)
        {
          float f;
          if (activeMemberNewValue != null
            && float.TryParse(activeMemberNewValue.ToString(), out f))
          {
            activeMember.SetValue(f);
          }
        }

        // Reset variables
        activeMember = null;
        activeMemberFieldType = FieldType.Normal;
        activeMemberNewValue = null;
      }

      if (layerTextInputActive && Event.current.keyCode == KeyCode.Return)
      {
        int i;
        if (int.TryParse(layerTextInputNewValue, out i))
        {
          SelectedGameObject.layer = i;
          layerTextInputActive = false;
        }
      }
    }

    private bool tagExpanded, layerExpanded;
    public void Display()
    {
      float panelWidth = Elements.Settings.InspectorPanelWidth;

      GUILayout.BeginHorizontal(GUILayout.Width(panelWidth),
        GUILayout.ExpandWidth(false));

      GUILayout.BeginVertical();

      if (IsGameObjectSelected)
      {
        // Text field to change game object's name
        SelectedGameObject.name = GUILayout.TextField(SelectedGameObject.name,
          Elements.InputFields.ThinNoTopBotMargin, GUILayout.Width(panelWidth),
          GUILayout.ExpandWidth(false));
      }
      else
      {
        GUILayout.TextField("Select a game object in the hierarchy",
          Elements.InputFields.ThinNoTopBotMargin, GUILayout.Width(panelWidth),
          GUILayout.ExpandWidth(false));
      }

      inspectorScroll = GUILayout.BeginScrollView(inspectorScroll,
        GUILayout.Width(panelWidth), GUILayout.ExpandWidth(false));

      if (IsGameObjectSelected)
      {
        GUILayout.BeginHorizontal();
        if (Elements.Tools.DoCollapseArrow(tagExpanded, false))
          tagExpanded = !tagExpanded;
        GUILayout.Label("Tag: " + SelectedGameObject.tag,
          Elements.Labels.LogEntry, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (Elements.Tools.DoCollapseArrow(layerExpanded))
          layerExpanded = !layerExpanded;
        GUILayout.Label("Layer: " + SelectedGameObject.layer,
          Elements.Labels.LogEntry, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        if (layerExpanded)
        {
          if (layerTextInputActive)
          {
            layerTextInputNewValue = GUILayout.TextField(
              layerTextInputNewValue, Elements.InputFields.ComponentField);
          }
          else
          {
            string newLayer = GUILayout.TextField(
              SelectedGameObject.layer.ToString(),
              Elements.InputFields.ComponentField);
            if (newLayer != SelectedGameObject.layer.ToString())
            {
              layerTextInputActive = true;
              layerTextInputNewValue = newLayer;
            }
          }
        }
      }

      GUILayout.Label("Components:", Elements.Labels.Title);

      foreach (var entry in new HashSet<ComponentEntry>(entries))
      {
        if (entry.Component == null)
        {
          entries.Remove(entry);
          continue;
        }
        DisplayComponent(entry);
      }

      GUILayout.EndScrollView();

      GUILayout.EndVertical();

      GUILayout.BeginVertical();
      DisplayFilters();
      DisplayUtilityButtons();
      GUILayout.EndVertical();

      GUILayout.EndHorizontal();
    }

    private void DisplayFilters()
    {
      foreach (var pair in new Dictionary<string, bool>(filter))
      {
        var style = pair.Value ? Elements.Buttons.Default
                               : Elements.Buttons.Disabled;
        if (GUILayout.Button(pair.Key, style))
        {
          filter[pair.Key] = !filter[pair.Key];
        }
      }
    }

    private void DisplayUtilityButtons()
    {
      var autoUpdate = Configuration.GetBool("objExpAutoUpdate", false);
      var style = autoUpdate ? Elements.Buttons.Default
                             : Elements.Buttons.Disabled;

      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Destroy"))
      {
        Destroy(SelectedGameObject);
      }
      if (GUILayout.Button("Focus"))
      {
        var mo = MouseOrbit.Instance;
        if (mo != null && IsGameObjectSelected)
          mo.target = SelectedGameObject.transform;
      }
      if (GUILayout.Button("Select focused"))
      {
        var mo = MouseOrbit.Instance;
        if (mo != null && mo.target != null)
        {
          SelectedGameObject = mo.target.gameObject;
        }
      }
      if (GUILayout.Button("Auto Update", style))
      {
        Configuration.SetBool("objExpAutoUpdate", !autoUpdate);
      }
    }

    private void DisplayComponent(ComponentEntry entry)
    {
      GUILayout.BeginHorizontal();
      if (Elements.Tools.DoCollapseArrow(entry.IsExpanded))
      {
        entry.IsExpanded = !entry.IsExpanded;
      }
      GUILayout.TextField(entry.Component.GetType().Name,
        Elements.Labels.LogEntry);
      GUILayout.EndHorizontal();

      if (entry.IsExpanded)
      {
        ShowFields("Properties", entry.Properties);
        ShowFields("Fields", entry.Fields);
      }
    }

    private void ShowFields(string title, IEnumerable<MemberValue> fields)
    {
      GUILayout.BeginHorizontal();
      Elements.Tools.Indent();
      GUILayout.TextField(title, Elements.Labels.LogEntryTitle);
      GUILayout.EndHorizontal();

      bool hasDisplayedFields = false;
      foreach (var member in fields)
      {
        // Don't display obsolete members
        if (member.IsObsolete) continue;

        // Filters
        if (!filter["Static"] && member.IsStatic) continue;
        if (!filter["Instance"] && !member.IsStatic) continue;
        if (!filter["Public"] && member.IsPublic) continue;
        if (!filter["NonPublic"] && !member.IsPublic) continue;
        if (!filter["Inherited"] && member.IsInherited) continue;
        if (filter["Has Setter"] && !member.HasSetter) continue;

        hasDisplayedFields = true;

        GUILayout.BeginHorizontal();
        Elements.Tools.Indent();

        object value = member.GetValue();
        string name = member.Name;
        Type type = member.Type;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        bool canModifyType = IsSupported(value);
        if (Elements.Tools.DoCollapseArrow(member.IsExpanded, canModifyType))
        {
          member.IsExpanded = !member.IsExpanded;
        }

        var typeStyle = new GUIStyle(Elements.Labels.LogEntry)
        {
          fontStyle = FontStyle.Bold,
          normal = { textColor = Elements.Colors.TypeText }
        };
        var valueStyle = new GUIStyle(Elements.Labels.LogEntry)
        {
          normal = { textColor = Elements.Colors.DefaultText * .8f }
        };

        GUILayout.Label(type.Name, typeStyle, GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + name + ":", Elements.Labels.LogEntry,
          GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + (value == null ? "null" : value.ToString()),
          valueStyle, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        if (member.IsExpanded)
        {
          GUILayout.BeginHorizontal();
          Elements.Tools.Indent();

          if (value is string)
          {
            DoInputField(member, value as string, FieldType.Normal);
          }
          else if (value is bool)
          {
            if (GUILayout.Button(value.ToString(),
              Elements.Buttons.ComponentField, GUILayout.Width(60)))
            {
              member.SetValue(!(bool)value);
            }
          }
          else if (value is Vector3)
          {
            var vec3Value = (Vector3)value;

            // X
            GUILayout.BeginHorizontal();
            GUILayout.Label("X: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, vec3Value.x, FieldType.VectorX, 80);
            GUILayout.EndHorizontal();
            // Y
            GUILayout.BeginHorizontal();
            GUILayout.Label("Y: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, vec3Value.y, FieldType.VectorY, 80);
            GUILayout.EndHorizontal();
            // Z
            GUILayout.BeginHorizontal();
            GUILayout.Label("Z: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, vec3Value.z, FieldType.VectorZ, 80);
            GUILayout.EndHorizontal();
          }
          else if (value is Color)
          {
            var colorValue = (Color)value;

            // R
            GUILayout.BeginHorizontal();
            GUILayout.Label("R: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, colorValue.r, FieldType.ColorR, 80);
            GUILayout.EndHorizontal();
            // G
            GUILayout.BeginHorizontal();
            GUILayout.Label("G: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, colorValue.g, FieldType.ColorG, 80);
            GUILayout.EndHorizontal();
            // B
            GUILayout.BeginHorizontal();
            GUILayout.Label("B: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, colorValue.b, FieldType.ColorB, 80);
            GUILayout.EndHorizontal();
            // A
            GUILayout.BeginHorizontal();
            GUILayout.Label("A: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, colorValue.a, FieldType.ColorA, 80);
            GUILayout.EndHorizontal();
          }
         else if (value is Quaternion)
          {
            var quatValue = (Quaternion)value;

            // X
            GUILayout.BeginHorizontal();
            GUILayout.Label("X: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, quatValue.x, FieldType.ColorR, 80);
            GUILayout.EndHorizontal();
            // Y
            GUILayout.BeginHorizontal();
            GUILayout.Label("Y: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, quatValue.y, FieldType.ColorG, 80);
            GUILayout.EndHorizontal();
            // Z
            GUILayout.BeginHorizontal();
            GUILayout.Label("Z: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, quatValue.z, FieldType.ColorB, 80);
            GUILayout.EndHorizontal();
            // W
            GUILayout.BeginHorizontal();
            GUILayout.Label("W: ", Elements.Labels.LogEntry,
              GUILayout.ExpandWidth(false));
            DoInputField(member, quatValue.w, FieldType.ColorA, 80);
            GUILayout.EndHorizontal();
          }
          else if (value is int)
          {
            DoInputField(member, (int)value, FieldType.Normal);
          }
          else if (value is float)
          {
            DoInputField(member, (float)value, FieldType.Normal);
          }
          GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
      }

      // Display "None" if there were no fields displayed
      if (hasDisplayedFields) return;

      GUILayout.BeginHorizontal();
      Elements.Tools.Indent();
      GUILayout.Label("None");
      GUILayout.EndHorizontal();
    }

    private void DoInputField(MemberValue member, object value, FieldType type,
      float width = 0)
    {
      GUILayoutOption widthOption = width > 0 ? GUILayout.Width(width)
                                              : GUILayout.ExpandWidth(true);

      // If this one is selected
      if (activeMember == member && activeMemberFieldType == type)
      {
        GUI.SetNextControlName(FIELD_EDIT_INPUT_NAME + member.ID);
        activeMemberNewValue = GUILayout.TextField((string)activeMemberNewValue,
          Elements.InputFields.ComponentField, widthOption);
      }
      else
      {
        string oldValue = value.ToString();

        GUI.SetNextControlName(FIELD_EDIT_INPUT_NAME + member.ID);
        string newValue = GUILayout.TextField(oldValue.ToString(),
          Elements.InputFields.ComponentField, widthOption);

        // Input was changed
        if (oldValue != newValue)
        {
          // Set current member to the active one
          activeMember = member;
          activeMemberFieldType = type;
          activeMemberNewValue = newValue;
        }
      }
    }

    public bool IsSupported(object value)
    {
      return value is string || value is bool || value is Vector3
          || value is int || value is float || value is Color
          || value is Quaternion;
    }
  }
}
