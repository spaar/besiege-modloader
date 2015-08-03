using System;
using System.Collections.Generic;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class InspectorPanel : MonoBehaviour
  {
    // TODO: Sometimes when changing int values, all text fields break.
    //       Unity's GUI system is then of the opinion that the control key
    //       is held down and acts accordingly. To fix that, just press
    //       control. This bug should be fixed, but after a lot of trying
    //       around, I still have no idea how or why this happens.

    enum FieldType
    {
      Normal, VectorX, VectorY, VectorZ
    }

    private const string FIELD_EDIT_INPUT_NAME = "field_edit_input";

    private MemberValue activeMember;
    private FieldType activeMemberFieldType = FieldType.Normal;
    private object activeMemberNewValue;

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
          Vector3 vector3 = (Vector3)activeMember.GetValue();
          float v;
          if (activeMemberNewValue != null
            && float.TryParse(activeMemberNewValue.ToString(), out v))
          {
            if (activeMemberFieldType == FieldType.VectorX) vector3.x = v;
            else if (activeMemberFieldType == FieldType.VectorY) vector3.y = v;
            else if (activeMemberFieldType == FieldType.VectorZ) vector3.z = v;
          }
          activeMember.SetValue(vector3);
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
    }

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

      GUILayout.EndHorizontal();
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
        // Don't display members without setter and obsolete members
        if (!member.HasSetter || member.IsObsolete) return;

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
            var vec3Value = (Vector3) value;


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
          || value is int    || value is float;
    }
  }
}
