using System;
using System.Reflection;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  enum EntryType
  {
    Field,
    Property
  }

  public class MemberValue
  {
    private static int nextID;
    public Type Type { get { return entryType == EntryType.Field ? FieldInfo.FieldType : PropertyInfo.PropertyType; } }
    public bool IsObsolete { get { return Attribute.IsDefined(info, typeof(ObsoleteAttribute)); } }
    public string Name { get { return info.Name; } }
    public bool IsExpanded { get; set; }
    public object Dummy { get; set; }

    public bool IsPublic
    {
      get
      {
        if (entryType == EntryType.Field) return FieldInfo.IsPublic;
        return PropertyInfo.GetGetMethod(true).IsPublic;
      }
    }

    public bool IsStatic
    {
      get
      {
        if (entryType == EntryType.Field) return FieldInfo.IsStatic;
        return PropertyInfo.GetGetMethod(true).IsStatic;
      }
    }

    public bool IsInherited { get; private set; }

    public bool HasSetter
    {
      get
      {
        if (entryType == EntryType.Field) return true;
        return !ReferenceEquals(PropertyInfo.GetSetMethod(true), null);
      }
    }

    private FieldInfo FieldInfo { get { return info as FieldInfo; } }
    private PropertyInfo PropertyInfo { get { return info as PropertyInfo; } }

    public readonly int ID = nextID++;

    private readonly EntryType entryType;
    private readonly Component component;
    private readonly MemberInfo info;

    public MemberValue(Component component, PropertyInfo property, bool inherited)
      : this(component, EntryType.Property, property, inherited)
    { }

    public MemberValue(Component component, FieldInfo field, bool inherited)
      : this(component, EntryType.Field, field, inherited)
    { }

    private MemberValue(Component component, EntryType entryType,
      MemberInfo info, bool inherited)
    {
      this.component = component;
      this.entryType = entryType;
      this.info = info;

      Dummy = GetValue();
      IsInherited = inherited;
    }

    public void SetValue(object value)
    {
      Dummy = value;
      if (entryType == EntryType.Field)
      {
        FieldInfo.SetValue(component, value);
      }
      else
      {
        PropertyInfo.SetValue(component, value, null);
      }
    }

    public object GetValue()
    {
      return entryType == EntryType.Field ?
        FieldInfo.GetValue(component) :
        PropertyInfo.GetValue(component, null);
    }

  }
}
