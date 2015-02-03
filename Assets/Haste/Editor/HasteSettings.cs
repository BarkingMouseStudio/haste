using System;
using UnityEditor;

namespace Haste {

  public enum HasteSetting {
    Version,
    Enabled,
    UsageCount,
    IgnorePaths,
    Source
  }

  public delegate void SettingChangedHandler<T>(HasteSetting setting, T before, T after);

  public static class HasteSettings {

    public static event SettingChangedHandler<bool> ChangedBool;
    public static event SettingChangedHandler<int> ChangedInt;
    public static event SettingChangedHandler<string> ChangedString;

    public static bool Enabled {
      get {
        return HasteSettings.GetBool(HasteSetting.Enabled);
      }
      set {
        HasteSettings.SetBool(HasteSetting.Enabled, value);
      }
    }

    public static int UsageCount {
      get {
        return HasteSettings.GetInt(HasteSetting.UsageCount);
      }
      set {
        HasteSettings.SetInt(HasteSetting.UsageCount, value);
      }
    }

    public static string IgnorePaths {
      get {
        return HasteSettings.GetString(HasteSetting.IgnorePaths);
      }
      set {
        HasteSettings.SetString(HasteSetting.IgnorePaths, value);
      }
    }

    public static string Version {
      get {
        return HasteSettings.GetString(HasteSetting.Version);
      }
      set {
        HasteSettings.SetString(HasteSetting.Version, value);
      }
    }

    public static int GetInt(HasteSetting setting) {
      return EditorPrefs.GetInt(GetPrefKey(setting), 0);
    }

    public static void SetInt(HasteSetting setting, int value) {
      var original = GetInt(setting);
      EditorPrefs.SetInt(GetPrefKey(setting), value);
      if (original != value && ChangedInt != null) {
        ChangedInt(setting, original, value);
      }
    }

    public static bool GetBool(HasteSetting setting) {
      return EditorPrefs.GetBool(GetPrefKey(setting), true);
    }

    public static void SetBool(HasteSetting setting, bool value) {
      var original = GetBool(setting);
      EditorPrefs.SetBool(GetPrefKey(setting), value);
      if (original != value && ChangedBool != null) {
        ChangedBool(setting, original, value);
      }
    }

    public static string GetString(HasteSetting setting) {
      return EditorPrefs.GetString(GetPrefKey(setting), "");
    }

    public static void SetString(HasteSetting setting, string value) {
      var original = GetString(setting);
      EditorPrefs.SetString(GetPrefKey(setting), value);
      if (original != value && ChangedString != null) {
        ChangedString(setting, original, value);
      }
    }

    public static string GetPrefKey(HasteSetting setting) {
      return String.Format("Haste:{0}", setting.ToString());
    }

    public static string GetPrefKey(HasteSetting setting, string value) {
      return String.Format("Haste:{0}:{1}", setting, value);
    }
  }
}