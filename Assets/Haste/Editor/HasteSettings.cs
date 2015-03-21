using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace Haste {

  public enum HasteSetting {
    Version,
    Enabled,
    IgnorePaths,
    UsageCount,
    UsageSince,
    Source
  }

  public delegate void SettingChangedHandler<T>(HasteSetting setting, T before, T after);

  public static class HasteSettings {

    static IDictionary<HasteSetting, string> keys = new Dictionary<HasteSetting, string>();

    public static event SettingChangedHandler<bool> ChangedBool;
    public static event SettingChangedHandler<int> ChangedInt;
    public static event SettingChangedHandler<string> ChangedString;

    public static int UsageAverage {
      get {
        TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - UsageSinceDate.Ticks);
        var days = Math.Max(1, Math.Floor(elapsed.TotalDays));
        return (int)(UsageCount / days);
      }
    }

    public static DateTime UsageSinceDate {
      get {
        return new DateTime(UsageSince);
      }
    }

    public static long UsageSince {
      get {
        var str = HasteSettings.GetString(HasteSetting.UsageSince);
        if (String.IsNullOrEmpty(str)) {
          return 0L;
        }
        return Convert.ToInt64(str);
      }
      set {
        HasteSettings.SetString(HasteSetting.UsageSince, value.ToString());
      }
    }

    public static bool Enabled {
      get {
        return HasteSettings.GetBool(HasteSetting.Enabled, true);
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

    public static int GetInt(HasteSetting setting, int defaultValue = 0) {
      return EditorPrefs.GetInt(GetPrefKey(setting), defaultValue);
    }

    public static void SetInt(HasteSetting setting, int value) {
      var original = GetInt(setting);
      EditorPrefs.SetInt(GetPrefKey(setting), value);
      if (original != value && ChangedInt != null) {
        ChangedInt(setting, original, value);
      }
    }

    public static bool GetBool(HasteSetting setting, bool defaultValue = false) {
      return EditorPrefs.GetBool(GetPrefKey(setting), defaultValue);
    }

    public static void SetBool(HasteSetting setting, bool value) {
      var original = GetBool(setting);
      EditorPrefs.SetBool(GetPrefKey(setting), value);
      if (original != value && ChangedBool != null) {
        ChangedBool(setting, original, value);
      }
    }

    public static string GetString(HasteSetting setting, string defaultValue = "") {
      return EditorPrefs.GetString(GetPrefKey(setting), defaultValue);
    }

    public static void SetString(HasteSetting setting, string value) {
      var original = GetString(setting);
      EditorPrefs.SetString(GetPrefKey(setting), value);
      if (original != value && ChangedString != null) {
        ChangedString(setting, original, value);
      }
    }

    public static string GetPrefKey(HasteSetting setting) {
      string key;
      if (!keys.TryGetValue(setting, out key)) {
        key = String.Format("Haste:{0}", setting.ToString());
        keys.Add(setting, key);
      }
      return key;
    }

    public static string GetPrefKey(HasteSetting setting, string value) {
      StringBuilder builder = new StringBuilder(GetPrefKey(setting));
      builder.Append(":").Append(value);
      return builder.ToString();
    }
  }
}