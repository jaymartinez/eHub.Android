using eHub.Android.Models;
using eHub.Common.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace eHub.Android
{
    public static class Settings
    {
        static ISettings AppSettings => CrossSettings.Current;

        const string SettingsKey = "settings_key";
        static readonly string SettingsDefault = string.Empty;

        const string CurrentPoolLightModeKey = "pool_light_mode";

        public static LightModeType CurrentPoolLightMode
        {
            get => (LightModeType)AppSettings.GetValueOrDefault(CurrentPoolLightModeKey, (int)LightModeType.NotSet);
            set => AppSettings.AddOrUpdateValue(CurrentPoolLightModeKey, (int)value);
        }
    }
}